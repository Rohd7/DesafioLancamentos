# 💳 Sistema de Lançamentos

Este projeto é um sistema completo de registro e consulta de transações financeiras, composto por:

- API (entrada de dados)
- Worker (processamento assíncrono)
- Banco SQLite
- Frontend (interface do usuário)

---

# 🧠 Arquitetura

O sistema segue um modelo **assíncrono baseado em eventos**, simulando uma arquitetura real de produção.

## 🔄 Fluxo

1. O usuário envia uma transação pelo Frontend
2. A API recebe a requisição e publica um evento na fila
3. O Worker consome esse evento
4. O Worker processa e salva no banco
5. A API permite consulta via GET

---

# Projeto Lançamentos/
│
├── api-lancamentos/              # API (ASP.NET)
├── api-lancamentos.tests/        # Testes da API
│
├── worker-consolidado/           # Worker (BackgroundService)
├── worker-consolidado.tests/     # Testes do Worker
│
├── sistema-lancamentos-site/     # Frontend (Node + HTML/JS)
│
└── database.db                   # Banco SQLite

---

# ⚙️ Adaptações para ambiente de teste

Para simplificar o projeto e torná-lo executável localmente, algumas adaptações foram feitas:

## 🔐 Autenticação

- Não há autenticação real
- Front usa `localStorage` para simular login

---

## 📩 Fila (SQS)

- Foi substituída por um **repositório local**
- A API grava mensagens no banco
- O Worker lê desse banco como se fosse uma fila

---

## 🗄 Banco de Dados

- SQLite (`database.db`)
- Arquivo local, sem necessidade de servidor
- Datamesh simulado com geração de arquivo .parquet na pasta datamesh dentro de worker-consolidado

---

## ☁️ Infraestrutura

- Não usa AWS, Azure ou serviços externos
- Tudo roda localmente

---

# 🧰 Pré-requisitos

Você precisa ter instalado:

## ✔️ .NET SDK
👉 https://dotnet.microsoft.com/download

Verificar:
dotnet --version


---

## ✔️ Node.js
👉 https://nodejs.org

Verificar:

node -v
npm -v


---

## ✔️ VS Code ()
👉 https://code.visualstudio.com

Extensões recomendadas:
- C#
- REST Client (opcional)

---

# 🚀 Como executar o projeto

## 🟦 1. Subir a API

### 📁 Caminho:
api-lancamentos


### ▶️ Executar:
dotnet run


### 🌐 URL:

http://localhost:5099/swagger


---

## 🟪 2. Subir o Worker

### 📁 Caminho:

worker-consolidado


### ▶️ Executar:

dotnet run


### 📌 O que ele faz:
- Fica rodando em loop
- Lê mensagens da fila
- Processa e salva no banco

---

## 🟨 3. Subir o Frontend

### 📁 Caminho:

sistema-lancamentos-site


### ▶️ Executar:


npm install
node server.js


### 🌐 Acessar:

http://localhost:3000


---

# 🧪 Como usar o sistema

## 📝 Criar lançamento

1. Acesse o Front
2. Faça login (simulado)
3. Preencha os dados
4. Clique em "Enviar"

---

## 🔄 Fluxo interno


Front → API → Fila → Worker → Banco


---

## 📊 Consultar histórico

1. Vá na aba "Histórico"
2. Use filtros:
   - Data
   - Tipo
   - Conta
3. Clique em "Buscar"

---

cd api-lancamentos.tests
dotnet test

Worker

cd worker-consolidado.tests
dotnet test

🧠 Conceitos aplicados
Arquitetura orientada a eventos
Processamento assíncrono
Separação de responsabilidades
DTOs e Contracts
Testes unitários com Mock
Front desacoplado da API
🚀 Possíveis melhorias
Autenticação real (JWT)
Paginação
Dashboard com gráficos
Deploy em cloud
Uso de fila real (SQS/RabbitMQ)

✅ Status do Projeto

✔ API funcionando
✔ Worker processando
✔ Front integrado
✔ Filtros implementados
✔ Testes unitários funcionando
