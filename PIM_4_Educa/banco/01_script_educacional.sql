/* =====================================================================
   PIM IV - Plataforma Educacional
   Banco: SQL Server
   Pode ser executado inteiro e mais de uma vez: ele recria tudo.
   Ordem: 1 limpeza | 2 tabelas | 3 indices | 4 dados iniciais
          5 procedures | 6 triggers | 7 consultas de teste
   ===================================================================== */

IF DB_ID(N'EducacionalDB') IS NULL
    CREATE DATABASE EducacionalDB;
GO

USE EducacionalDB;
GO

/* ---------------------------------------------------------------------
   1. LIMPEZA
   --------------------------------------------------------------------- */
DROP PROCEDURE IF EXISTS dbo.sp_BoletimAluno;
DROP PROCEDURE IF EXISTS dbo.sp_MediaTurma;
DROP PROCEDURE IF EXISTS dbo.sp_AtividadesPendentes;
DROP PROCEDURE IF EXISTS dbo.sp_LancarNota;

DROP TABLE IF EXISTS dbo.LogAuditoria;
DROP TABLE IF EXISTS dbo.Material;
DROP TABLE IF EXISTS dbo.Aviso;
DROP TABLE IF EXISTS dbo.Entrega;
DROP TABLE IF EXISTS dbo.Atividade;
DROP TABLE IF EXISTS dbo.Matricula;
DROP TABLE IF EXISTS dbo.Turma;
DROP TABLE IF EXISTS dbo.Disciplina;
DROP TABLE IF EXISTS dbo.Usuario;
GO

/* ---------------------------------------------------------------------
   2. TABELAS
   --------------------------------------------------------------------- */
CREATE TABLE dbo.Usuario (
    Id        INT IDENTITY(1,1) NOT NULL,
    Nome      NVARCHAR(120)     NOT NULL,
    Email     NVARCHAR(150)     NOT NULL,
    SenhaHash NVARCHAR(200)     NOT NULL,
    Perfil    VARCHAR(10)       NOT NULL,
    Ativo     BIT               NOT NULL CONSTRAINT DF_Usuario_Ativo DEFAULT 1,
    CriadoEm  DATETIME2         NOT NULL CONSTRAINT DF_Usuario_CriadoEm DEFAULT SYSDATETIME(),
    CONSTRAINT PK_Usuario PRIMARY KEY (Id),
    CONSTRAINT UQ_Usuario_Email UNIQUE (Email),
    CONSTRAINT CK_Usuario_Perfil CHECK (Perfil IN ('Aluno', 'Professor', 'Admin'))
);

CREATE TABLE dbo.Disciplina (
    Id        INT IDENTITY(1,1) NOT NULL,
    Nome      NVARCHAR(100)     NOT NULL,
    Descricao NVARCHAR(300)     NULL,
    CONSTRAINT PK_Disciplina PRIMARY KEY (Id),
    CONSTRAINT UQ_Disciplina_Nome UNIQUE (Nome)
);

CREATE TABLE dbo.Turma (
    Id           INT IDENTITY(1,1) NOT NULL,
    DisciplinaId INT               NOT NULL,
    ProfessorId  INT               NOT NULL,
    Nome         NVARCHAR(60)      NOT NULL,
    Periodo      VARCHAR(10)       NOT NULL,
    CONSTRAINT PK_Turma PRIMARY KEY (Id),
    CONSTRAINT FK_Turma_Disciplina FOREIGN KEY (DisciplinaId) REFERENCES dbo.Disciplina (Id),
    CONSTRAINT FK_Turma_Professor  FOREIGN KEY (ProfessorId)  REFERENCES dbo.Usuario (Id)
);

CREATE TABLE dbo.Matricula (
    Id            INT IDENTITY(1,1) NOT NULL,
    TurmaId       INT               NOT NULL,
    AlunoId       INT               NOT NULL,
    DataMatricula DATETIME2         NOT NULL CONSTRAINT DF_Matricula_Data DEFAULT SYSDATETIME(),
    CONSTRAINT PK_Matricula PRIMARY KEY (Id),
    CONSTRAINT UQ_Matricula_TurmaAluno UNIQUE (TurmaId, AlunoId),
    CONSTRAINT FK_Matricula_Turma FOREIGN KEY (TurmaId) REFERENCES dbo.Turma (Id),
    CONSTRAINT FK_Matricula_Aluno FOREIGN KEY (AlunoId) REFERENCES dbo.Usuario (Id)
);

CREATE TABLE dbo.Atividade (
    Id        INT IDENTITY(1,1) NOT NULL,
    TurmaId   INT               NOT NULL,
    Titulo    NVARCHAR(150)     NOT NULL,
    Descricao NVARCHAR(1000)    NULL,
    Prazo     DATETIME2         NOT NULL,
    CONSTRAINT PK_Atividade PRIMARY KEY (Id),
    CONSTRAINT FK_Atividade_Turma FOREIGN KEY (TurmaId) REFERENCES dbo.Turma (Id)
);

CREATE TABLE dbo.Entrega (
    Id          INT IDENTITY(1,1) NOT NULL,
    AtividadeId INT               NOT NULL,
    AlunoId     INT               NOT NULL,
    Conteudo    NVARCHAR(MAX)     NOT NULL,
    DataEnvio   DATETIME2         NOT NULL CONSTRAINT DF_Entrega_DataEnvio DEFAULT SYSDATETIME(),
    Nota        DECIMAL(4,2)      NULL,
    CONSTRAINT PK_Entrega PRIMARY KEY (Id),
    CONSTRAINT UQ_Entrega_AtividadeAluno UNIQUE (AtividadeId, AlunoId),
    CONSTRAINT CK_Entrega_Nota CHECK (Nota IS NULL OR (Nota >= 0 AND Nota <= 10)),
    CONSTRAINT FK_Entrega_Atividade FOREIGN KEY (AtividadeId) REFERENCES dbo.Atividade (Id),
    CONSTRAINT FK_Entrega_Aluno     FOREIGN KEY (AlunoId)     REFERENCES dbo.Usuario (Id)
);

CREATE TABLE dbo.Aviso (
    Id             INT IDENTITY(1,1) NOT NULL,
    TurmaId        INT               NOT NULL,
    AutorId        INT               NOT NULL,
    Texto          NVARCHAR(1000)    NOT NULL,
    DataPublicacao DATETIME2         NOT NULL CONSTRAINT DF_Aviso_Data DEFAULT SYSDATETIME(),
    CONSTRAINT PK_Aviso PRIMARY KEY (Id),
    CONSTRAINT FK_Aviso_Turma FOREIGN KEY (TurmaId) REFERENCES dbo.Turma (Id),
    CONSTRAINT FK_Aviso_Autor FOREIGN KEY (AutorId) REFERENCES dbo.Usuario (Id)
);

CREATE TABLE dbo.Material (
    Id           INT IDENTITY(1,1) NOT NULL,
    DisciplinaId INT               NOT NULL,
    Titulo       NVARCHAR(150)     NOT NULL,
    Url          NVARCHAR(400)     NOT NULL,
    CONSTRAINT PK_Material PRIMARY KEY (Id),
    CONSTRAINT FK_Material_Disciplina FOREIGN KEY (DisciplinaId) REFERENCES dbo.Disciplina (Id)
);

CREATE TABLE dbo.LogAuditoria (
    Id         BIGINT IDENTITY(1,1) NOT NULL,
    Tabela     VARCHAR(50)          NOT NULL,
    RegistroId INT                  NOT NULL,
    Acao       VARCHAR(30)          NOT NULL,
    Detalhe    NVARCHAR(400)        NULL,
    DataHora   DATETIME2            NOT NULL CONSTRAINT DF_Log_DataHora DEFAULT SYSDATETIME(),
    CONSTRAINT PK_LogAuditoria PRIMARY KEY (Id)
);
GO

/* ---------------------------------------------------------------------
   3. INDICES (acelerar as consultas mais usadas pela API)
   --------------------------------------------------------------------- */
CREATE INDEX IX_Turma_Professor   ON dbo.Turma     (ProfessorId);
CREATE INDEX IX_Matricula_Aluno   ON dbo.Matricula (AlunoId);
CREATE INDEX IX_Atividade_Turma   ON dbo.Atividade (TurmaId, Prazo);
CREATE INDEX IX_Entrega_Aluno     ON dbo.Entrega   (AlunoId);
CREATE INDEX IX_Aviso_Turma       ON dbo.Aviso     (TurmaId, DataPublicacao DESC);
GO

/* ---------------------------------------------------------------------
   4. DADOS INICIAIS
   A SenhaHash 'TROCAR' e substituida pela API na primeira execucao
   (senha de todos os usuarios de teste: Senha@123).
   --------------------------------------------------------------------- */
INSERT INTO dbo.Usuario (Nome, Email, SenhaHash, Perfil) VALUES
 (N'Administrador',      N'admin@escola.com',          N'TROCAR', 'Admin'),     -- Id 1
 (N'Profa. Ana Souza',   N'ana.souza@escola.com',      N'TROCAR', 'Professor'), -- Id 2
 (N'Prof. Carlos Lima',  N'carlos.lima@escola.com',    N'TROCAR', 'Professor'), -- Id 3
 (N'João Pereira',       N'joao.pereira@escola.com',   N'TROCAR', 'Aluno'),     -- Id 4
 (N'Maria Santos',       N'maria.santos@escola.com',   N'TROCAR', 'Aluno'),     -- Id 5
 (N'Pedro Alves',        N'pedro.alves@escola.com',    N'TROCAR', 'Aluno'),     -- Id 6
 (N'Beatriz Nunes',      N'beatriz.nunes@escola.com',  N'TROCAR', 'Aluno');     -- Id 7

INSERT INTO dbo.Disciplina (Nome, Descricao) VALUES
 (N'Matemática',        N'Fundamentos de matemática.'),     -- Id 1
 (N'Língua Portuguesa', N'Leitura, interpretação e escrita.'), -- Id 2
 (N'Ciências',          N'Introdução às ciências naturais.');  -- Id 3

INSERT INTO dbo.Turma (DisciplinaId, ProfessorId, Nome, Periodo) VALUES
 (1, 2, N'Matemática - 1A',  '2026/2'),  -- Id 1
 (2, 2, N'Português - 1A',   '2026/2'),  -- Id 2
 (3, 3, N'Ciências - 1A',    '2026/2');  -- Id 3

INSERT INTO dbo.Matricula (TurmaId, AlunoId) VALUES
 (1, 4), (1, 5), (1, 6),
 (2, 4), (2, 5),
 (3, 4), (3, 6), (3, 7);

INSERT INTO dbo.Atividade (TurmaId, Titulo, Descricao, Prazo) VALUES
 (1, N'Lista de exercícios 1',     N'Resolver os exercícios 1 a 10.',   DATEADD(DAY, -5,  SYSDATETIME())), -- Id 1 (encerrada)
 (1, N'Prova online - Frações',    N'Responder ao questionário.',       DATEADD(DAY, 10,  SYSDATETIME())), -- Id 2
 (2, N'Redação: Meu bairro',       N'Texto de 20 linhas.',              DATEADD(DAY, -3,  SYSDATETIME())), -- Id 3 (encerrada)
 (2, N'Leitura do capítulo 3',     N'Entregar um resumo.',              DATEADD(DAY, 14,  SYSDATETIME())), -- Id 4
 (3, N'Relatório do experimento',  N'Descrever o experimento feito.',   DATEADD(DAY, 7,   SYSDATETIME())); -- Id 5

INSERT INTO dbo.Entrega (AtividadeId, AlunoId, Conteudo, DataEnvio, Nota) VALUES
 (1, 4, N'Resolução dos exercícios.', DATEADD(DAY, -7, SYSDATETIME()), 8.50),
 (1, 5, N'Resolução dos exercícios.', DATEADD(DAY, -6, SYSDATETIME()), 9.00),
 (1, 6, N'Resolução dos exercícios.', DATEADD(DAY, -6, SYSDATETIME()), 7.00),
 (3, 4, N'Texto da redação.',         DATEADD(DAY, -4, SYSDATETIME()), 9.50),
 (3, 5, N'Texto da redação.',         DATEADD(DAY, -4, SYSDATETIME()), NULL),
 (5, 4, N'Relatório do experimento.', DATEADD(DAY, -1, SYSDATETIME()), NULL);

INSERT INTO dbo.Aviso (TurmaId, AutorId, Texto) VALUES
 (1, 2, N'A prova online abre na próxima semana.'),
 (2, 2, N'Lembrem de revisar a redação antes de entregar.'),
 (3, 3, N'Tragam o jaleco na próxima aula.');

INSERT INTO dbo.Material (DisciplinaId, Titulo, Url) VALUES
 (1, N'Apostila de Frações', N'https://exemplo.com/fracoes'),
 (2, N'Guia de redação',     N'https://exemplo.com/redacao');
GO

/* ---------------------------------------------------------------------
   5. PROCEDURES
   --------------------------------------------------------------------- */

-- Boletim do aluno: media e andamento por turma
CREATE PROCEDURE dbo.sp_BoletimAluno
    @AlunoId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  t.Id                                  AS TurmaId,
            t.Nome                                AS Turma,
            d.Nome                                AS Disciplina,
            COUNT(DISTINCT a.Id)                  AS TotalAtividades,
            COUNT(DISTINCT e.Id)                  AS TotalEntregas,
            CAST(AVG(e.Nota) AS DECIMAL(4,2))     AS Media
    FROM dbo.Matricula m
    JOIN dbo.Turma t        ON t.Id = m.TurmaId
    JOIN dbo.Disciplina d   ON d.Id = t.DisciplinaId
    LEFT JOIN dbo.Atividade a ON a.TurmaId = t.Id
    LEFT JOIN dbo.Entrega e   ON e.AtividadeId = a.Id AND e.AlunoId = m.AlunoId
    WHERE m.AlunoId = @AlunoId
    GROUP BY t.Id, t.Nome, d.Nome
    ORDER BY d.Nome;
END
GO

-- Media dos alunos de uma turma (visao do professor)
CREATE PROCEDURE dbo.sp_MediaTurma
    @TurmaId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  u.Id                              AS AlunoId,
            u.Nome                            AS Aluno,
            COUNT(e.Id)                       AS Entregas,
            CAST(AVG(e.Nota) AS DECIMAL(4,2)) AS Media
    FROM dbo.Matricula m
    JOIN dbo.Usuario u        ON u.Id = m.AlunoId
    LEFT JOIN dbo.Atividade a ON a.TurmaId = m.TurmaId
    LEFT JOIN dbo.Entrega e   ON e.AtividadeId = a.Id AND e.AlunoId = m.AlunoId
    WHERE m.TurmaId = @TurmaId
    GROUP BY u.Id, u.Nome
    ORDER BY Media DESC, u.Nome;
END
GO

-- Atividades que o aluno ainda pode entregar (prazo aberto e sem entrega)
CREATE PROCEDURE dbo.sp_AtividadesPendentes
    @AlunoId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  a.Id      AS AtividadeId,
            t.Id      AS TurmaId,
            t.Nome    AS Turma,
            d.Nome    AS Disciplina,
            a.Titulo  AS Titulo,
            a.Prazo   AS Prazo
    FROM dbo.Matricula m
    JOIN dbo.Turma t        ON t.Id = m.TurmaId
    JOIN dbo.Disciplina d   ON d.Id = t.DisciplinaId
    JOIN dbo.Atividade a    ON a.TurmaId = t.Id
    WHERE m.AlunoId = @AlunoId
      AND a.Prazo > SYSDATETIME()
      AND NOT EXISTS (SELECT 1 FROM dbo.Entrega e
                      WHERE e.AtividadeId = a.Id AND e.AlunoId = @AlunoId)
    ORDER BY a.Prazo;
END
GO

-- Lanca a nota de uma entrega, somente pelo professor da turma
CREATE PROCEDURE dbo.sp_LancarNota
    @EntregaId   INT,
    @Nota        DECIMAL(4,2),
    @ProfessorId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @Nota < 0 OR @Nota > 10
        THROW 50010, N'A nota deve estar entre 0 e 10.', 1;

    IF NOT EXISTS (SELECT 1
                   FROM dbo.Entrega e
                   JOIN dbo.Atividade a ON a.Id = e.AtividadeId
                   JOIN dbo.Turma t     ON t.Id = a.TurmaId
                   WHERE e.Id = @EntregaId AND t.ProfessorId = @ProfessorId)
        THROW 50011, N'Entrega não encontrada ou não pertence a uma turma deste professor.', 1;

    UPDATE dbo.Entrega SET Nota = @Nota WHERE Id = @EntregaId;
END
GO

/* ---------------------------------------------------------------------
   6. TRIGGERS
   --------------------------------------------------------------------- */

-- Regra: nao aceitar entrega depois do prazo
CREATE TRIGGER dbo.trg_Entrega_ValidaPrazo
ON dbo.Entrega
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1
               FROM inserted i
               JOIN dbo.Atividade a ON a.Id = i.AtividadeId
               WHERE i.DataEnvio > a.Prazo)
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50001, N'O prazo desta atividade já encerrou.', 1;
    END
END
GO

-- Regra: somente usuarios com perfil Aluno podem ser matriculados
CREATE TRIGGER dbo.trg_Matricula_ValidaAluno
ON dbo.Matricula
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1
               FROM inserted i
               JOIN dbo.Usuario u ON u.Id = i.AlunoId
               WHERE u.Perfil <> 'Aluno')
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50002, N'Somente usuários com perfil Aluno podem ser matriculados.', 1;
    END
END
GO

-- Auditoria: registra toda alteracao de nota (valor antigo -> novo)
CREATE TRIGGER dbo.trg_Entrega_AuditaNota
ON dbo.Entrega
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF UPDATE(Nota)
    BEGIN
        INSERT INTO dbo.LogAuditoria (Tabela, RegistroId, Acao, Detalhe)
        SELECT  'Entrega',
                i.Id,
                'ALTERACAO_NOTA',
                CONCAT(N'Nota: ',
                       ISNULL(CAST(d.Nota AS NVARCHAR(10)), N'(vazia)'),
                       N' -> ',
                       ISNULL(CAST(i.Nota AS NVARCHAR(10)), N'(vazia)'))
        FROM inserted i
        JOIN deleted  d ON d.Id = i.Id
        WHERE ISNULL(i.Nota, -1) <> ISNULL(d.Nota, -1);
    END
END
GO

/* ---------------------------------------------------------------------
   7. CONSULTAS DE TESTE (descomente para experimentar)
   --------------------------------------------------------------------- */
-- EXEC dbo.sp_BoletimAluno @AlunoId = 4;
-- EXEC dbo.sp_MediaTurma @TurmaId = 1;
-- EXEC dbo.sp_AtividadesPendentes @AlunoId = 4;
-- EXEC dbo.sp_LancarNota @EntregaId = 5, @Nota = 8.0, @ProfessorId = 2;  -- dispara o trigger de auditoria
-- SELECT * FROM dbo.LogAuditoria;
-- INSERT INTO dbo.Entrega (AtividadeId, AlunoId, Conteudo) VALUES (1, 7, N'teste'); -- falha: prazo encerrado / nao matriculado
GO
