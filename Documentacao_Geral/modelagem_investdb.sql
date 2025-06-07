-- Script de criação do banco de dados investdb

-- Usuários
CREATE TABLE usuarios (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    perc_corretagem DECIMAL(5,2) NOT NULL,
    senha VARCHAR(255) NOT NULL
);

-- Ativos
CREATE TABLE ativos (
    id INT AUTO_INCREMENT PRIMARY KEY,
    codigo VARCHAR(10) NOT NULL UNIQUE,
    nome VARCHAR(100) NOT NULL,
    preco_atual DECIMAL(15,4)
);

-- Operações
CREATE TABLE operacoes (
    id INT AUTO_INCREMENT PRIMARY KEY,
    usuario_id INT NOT NULL,
    ativo_id INT NOT NULL,
    quantidade INT NOT NULL,
    preco_unitario DECIMAL(15,4) NOT NULL,
    tipo_operacao ENUM('COMPRA', 'VENDA') NOT NULL,
    corretagem DECIMAL(10,2) NOT NULL,
    data_hora DATETIME NOT NULL,
    FOREIGN KEY (usuario_id) REFERENCES usuarios(id),
    FOREIGN KEY (ativo_id) REFERENCES ativos(id)
);

-- Cotações
CREATE TABLE cotacoes (
    id INT AUTO_INCREMENT PRIMARY KEY,
    ativo_id INT NOT NULL,
    preco_unitario DECIMAL(15,4) NOT NULL,
    data_hora DATETIME NOT NULL,
    FOREIGN KEY (ativo_id) REFERENCES ativos(id)
);

-- Posições
CREATE TABLE posicoes (
    id INT AUTO_INCREMENT PRIMARY KEY,
    usuario_id INT NOT NULL,
    ativo_id INT NOT NULL,
    ticker VARCHAR(10) NOT NULL,
    quantidade_liquida INT NOT NULL,
    preco_medio DECIMAL(15,4) NOT NULL,
    valor_mercado DECIMAL(15,4) NOT NULL,
    pnl DECIMAL(15,4) NOT NULL,
    ultima_atualizacao DATETIME,
    FOREIGN KEY (usuario_id) REFERENCES usuarios(id),
    FOREIGN KEY (ativo_id) REFERENCES ativos(id)
); 