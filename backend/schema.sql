CREATE DATABASE investdb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE investdb;



CREATE TABLE usuarios (
  id INT AUTO_INCREMENT PRIMARY KEY,
  nome VARCHAR(100) NOT NULL,
  email VARCHAR(150) NOT NULL UNIQUE,
  perc_corretagem DECIMAL(5,2) NOT NULL  
);

CREATE TABLE ativos (
  id INT AUTO_INCREMENT PRIMARY KEY,
  codigo VARCHAR(10) NOT NULL UNIQUE,    -- ex.: ITSA3, KDIF11
  nome VARCHAR(100) NOT NULL,
  preco_atual DECIMAL(18,6) NOT NULL DEFAULT 0
);

CREATE TABLE operacoes (
  id INT AUTO_INCREMENT PRIMARY KEY,
  usuario_id INT NOT NULL,
  ativo_id INT NOT NULL,
  quantidade DECIMAL(18,6) NOT NULL,
  preco_unitario DECIMAL(18,6) NOT NULL,
  tipo_operacao ENUM('COMPRA','VENDA') NOT NULL,
  corretagem DECIMAL(12,2) NOT NULL,     -- valor fixo pago na corretora
  data_hora DATETIME NOT NULL,
  FOREIGN KEY (usuario_id) REFERENCES usuarios(id),
  FOREIGN KEY (ativo_id) REFERENCES ativos(id)
);

CREATE TABLE cotacoes (
  id INT AUTO_INCREMENT PRIMARY KEY,
  ativo_id INT NOT NULL,
  preco_unitario DECIMAL(18,6) NOT NULL,
  data_hora DATETIME NOT NULL,
  FOREIGN KEY (ativo_id) REFERENCES ativos(id),
  INDEX idx_cotacoes_ativo_datahora (ativo_id, data_hora)
);

CREATE TABLE posicao (
  id INT AUTO_INCREMENT PRIMARY KEY,
  usuario_id INT NOT NULL,
  ativo_id INT NOT NULL,
  quantidade DECIMAL(18,6) NOT NULL,
  preco_medio DECIMAL(18,6) NOT NULL,
  pnl DECIMAL(18,2) NOT NULL,            -- Profit & Loss
  FOREIGN KEY (usuario_id) REFERENCES usuarios(id),
  FOREIGN KEY (ativo_id) REFERENCES ativos(id),
  UNIQUE KEY uq_posicao_usuario_ativo (usuario_id, ativo_id)
);

select * from  usuarios;

-- Exemplo de usuário
INSERT INTO usuarios (nome, email, perc_corretagem)
VALUES ('Kaique Suzart', 'kaique@example.com', 0.50);

-- Exemplo de ativos
INSERT INTO ativos (codigo, nome)
VALUES ('ITSA3', 'Itaú SA'), 
       ('PETR4', 'Petrobras');

-- Exemplo de operações (compra e venda)
INSERT INTO operacoes (usuario_id, ativo_id, quantidade, preco_unitario, tipo_operacao, corretagem, data_hora)
VALUES 
  (1, 1, 100.000000, 10.500000, 'COMPRA', 5.00, '2025-06-01 10:00:00'),
  (1, 1,  50.000000, 11.000000, 'COMPRA', 5.00, '2025-06-02 11:00:00'),
  (1, 2, 200.000000, 25.000000, 'COMPRA', 7.50, '2025-06-01 14:00:00'),
  (1, 1,  30.000000, 12.000000, 'VENDA', 5.00, '2025-06-03 09:00:00');

-- Cotação atual de cada ativo
INSERT INTO cotacoes (ativo_id, preco_unitario, data_hora)
VALUES 
  (1, 11.500000, '2025-06-03 15:00:00'),
  (2, 26.000000, '2025-06-03 15:00:00');


