# PIM IV – Plataforma Educacional

Sistema para professores e alunos: turmas, atividades, entregas, notas, avisos e materiais.
Três partes que conversam entre si:

```
PIM_4_Educa/
 ├─ banco/01_script_educacional.sql   ← script único do banco (SQL Server)
 ├─ src/
 │   ├─ Educacional.Domain/           ← entidades (orientação a objetos)
 │   ├─ Educacional.Application/      ← regras de negócio, serviços, DTOs, interfaces
 │   ├─ Educacional.Infrastructure/   ← EF Core, repositórios, procedures, BCrypt
 │   ├─ Educacional.Api/              ← API REST + JWT + Swagger   (porta 5080)
 │   └─ Educacional.Web/              ← site ASP.NET Core MVC       (porta 5090)
 └─ mobile/                           ← app Android (Java + XML) – criar no Android Studio
```

Fluxo: **Site e App Android → API → Banco**. O site e o app usam a mesma API e o mesmo banco.
Dependência entre camadas: `Api → Application ← Infrastructure`, `Application → Domain`.
O site (`Web`) usa os DTOs da camada Application como "contrato" com a API.

## Como rodar

1. **Banco:** no VS Code (extensão *SQL Server (mssql)*), conecte ao SQL Server, abra
   `banco/01_script_educacional.sql` e execute o arquivo inteiro (cria o `EducacionalDB`).
2. **Connection string:** ajuste `src/Educacional.Api/appsettings.json`.
   - Login do Windows (padrão do arquivo), ou
   - `Server=localhost,1433;Database=EducacionalDB;User Id=sa;Password=SUA_SENHA;TrustServerCertificate=True;`
3. **Solução** (uma vez, na pasta `PIM_4_Educa`):
   ```
   dotnet new sln -n PIM_4_Educa
   dotnet sln add src/Educacional.Domain src/Educacional.Application src/Educacional.Infrastructure src/Educacional.Api src/Educacional.Web
   ```
4. **Executar** (dois terminais, a API precisa estar no ar antes do site):
   ```
   dotnet run --project src/Educacional.Api     # http://localhost:5080/swagger
   dotnet run --project src/Educacional.Web     # http://localhost:5090
   ```
   O endereço da API usado pelo site fica em `src/Educacional.Web/appsettings.json` (`Api:BaseUrl`).

## Contas de teste (senha de todas: `Senha@123`)

| Perfil | E-mail |
|---|---|
| Admin | admin@escola.com |
| Professor | ana.souza@escola.com (Matemática e Português), carlos.lima@escola.com (Ciências) |
| Aluno | joao.pereira@escola.com, maria.santos@escola.com, pedro.alves@escola.com, beatriz.nunes@escola.com |

Na primeira execução a API troca o `SenhaHash = 'TROCAR'` do script por um hash BCrypt real.

## Funcionalidades do site

| Perfil | O que faz |
|---|---|
| Todos | Login/logout, painel inicial, mural de avisos e materiais da turma |
| Aluno | Minhas turmas, atividades pendentes, enviar entrega, ver nota, boletim |
| Professor | Minhas turmas, criar atividade, ver entregas e lançar nota, média da turma, publicar aviso e material, lista de alunos |
| Admin | Cadastro de usuários (ativar/desativar), disciplinas, turmas e matrículas |

**Acessibilidade (Etapa 3):** link "pular para o conteúdo", `lang="pt-BR"`, rótulos em todos os campos,
tabelas com `caption`/`scope`, foco visível, navegação por teclado, botões **A-/A+** (tamanho do texto)
e **Alto contraste**, com a escolha lembrada entre as visitas.

**Segurança (Etapa 4):** senhas com BCrypt, JWT, perfis por rota, proteção CSRF (antiforgery) em todo POST,
cookie `HttpOnly`, textos dos usuários sempre escapados na tela, links de materiais só `http/https`,
mesma mensagem para "e-mail inexistente" e "senha errada".

## Endpoints da API (para o Swagger e para o app Android)

| Método | Rota | Perfil |
|---|---|---|
| POST | /api/auth/login | público |
| GET | /api/turmas/minhas | logado |
| GET | /api/turmas/{id} | membro da turma |
| GET | /api/turmas/{id}/atividades | membro da turma |
| GET | /api/turmas/{id}/avisos | membro da turma |
| GET | /api/turmas/{id}/materiais | membro da turma |
| GET | /api/turmas/{id}/alunos | Professor da turma / Admin |
| GET | /api/turmas/{id}/media | Professor da turma / Admin (`sp_MediaTurma`) |
| POST | /api/turmas/{id}/avisos | Professor da turma / Admin |
| POST | /api/turmas/{id}/materiais | Professor da turma / Admin |
| GET / POST | /api/turmas | Admin |
| POST | /api/atividades | Professor |
| GET | /api/atividades/pendentes | Aluno (`sp_AtividadesPendentes`) |
| GET | /api/atividades/{id} | membro da turma |
| GET | /api/atividades/{id}/entregas | Professor da turma / Admin |
| POST | /api/entregas | Aluno |
| PUT | /api/entregas/{id}/nota | Professor (`sp_LancarNota` + trigger de auditoria) |
| GET | /api/notas/boletim | Aluno (`sp_BoletimAluno`) |
| GET / POST | /api/usuarios · PUT /api/usuarios/{id}/ativo | Admin |
| GET / POST | /api/disciplinas | Admin |
| POST / DELETE | /api/matriculas | Admin |

## Regras no banco (Etapa 7)

- `trg_Entrega_ValidaPrazo`: bloqueia entrega após o prazo.
- `trg_Matricula_ValidaAluno`: só perfil Aluno pode ser matriculado.
- `trg_Entrega_AuditaNota`: grava em `LogAuditoria` toda mudança de nota.
- `sp_BoletimAluno`, `sp_MediaTurma`, `sp_AtividadesPendentes`, `sp_LancarNota`.

## Observações

- Este código foi escrito sem poder compilar no ambiente de geração. No primeiro `dotnet build`, se aparecer
  algum erro, copie a mensagem e peça o ajuste.
- A chave `Jwt:Key` do `appsettings.json` da API é só de desenvolvimento: troque por uma sua.
- O cookie do site expira em 120 min, igual ao JWT (`Jwt:ExpireMinutes`). Se mudar um, mude o outro.
- Pacotes com versões flutuantes `8.0.*` (.NET 8). Se a disciplina usa outra versão, altere o `TargetFramework`.
- Pendente: app Android (pasta `mobile/`), Docker e plano de CI/CD (Etapa 8), documento do PIM.
