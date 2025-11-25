# CShapAPI

Uma Web API simples desenvolvida em **C# / .NET 8**, construída para
demonstrar conceitos de API RESTful, arquitetura básica e práticas de
desenvolvimento com .NET.

## 🚀 Funcionalidades

-   Endpoints REST para operações básicas (CRUD)\
-   Estrutura de projeto clara (Controllers, Models, etc.)\
-   Uso de injeção de dependência (Dependency Injection)\
-   Serialização/Deserialização de JSON\
-   Logging básico\
-   Documentação via Swagger (se configurado)

## 🧰 Requisitos

-   .NET 8 SDK\
-   IDE (VS Code ou Visual Studio)

## 🗄️ Configuração do SQL Server

Para executar a API com SQL Server, siga estes passos:

1.  **Instale o SQL Server** (Developer ou Express) e o **SQL Server
    Management Studio (SSMS)**.\
2.  Crie um novo banco de dados (ex.: `MinhaAPI_DB`).\
3.  Copie a *connection string* no SSMS em:\
    `Propriedades > Connection String`.\
4.  No `appsettings.json`, configure sua string de conexão:

``` json
"ConnectionStrings": {
  "DefaultConnection": "Server=SEU_SERVIDOR;Database=MinhaAPI_DB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

5.  Execute as migrações (se estiver usando EF Core):\

``` bash
dotnet ef database update
```

Pronto! Sua API já está conectada ao SQL Server.


## Como executar localmente

``` bash
cd CShapAPI
dotnet restore
dotnet build
dotnet run --project API.csproj
```

Acesse:\
`https://localhost:5001/swagger`

## 📦 Endpoints (exemplos)

  Método   Rota                Descrição
  -------- ------------------- ------------------------
  GET      `/api/itens`        Retorna todos os itens
  GET      `/api/itens/{id}`   Retorna um item por ID
  POST     `/api/itens`        Cria um novo item
  PUT      `/api/itens/{id}`   Atualiza um item
  DELETE   `/api/itens/{id}`   Exclui um item

## 🌐 Como abrir o HTML em um servidor local no VS Code

Para visualizar seu arquivo **HTML** rodando em um servidor local no Visual Studio Code, siga estes passos:

1. Abra o projeto no **VS Code**.
2. Instale a extensão **Live Server** (por Ritwick Dey):
   - Vá em **Extensions (Ctrl+Shift+X)**  
   - Pesquise por **Live Server**  
   - Clique em **Install**
3. Abra o arquivo HTML que deseja visualizar.
4. Clique com o botão direito no arquivo e selecione:
   **“Open with Live Server”**
5. O navegador será aberto automaticamente em um endereço como:
```
http://127.0.0.1:5500/
```
6. Sempre que você alterar e salvar o arquivo, a página será recarregada automaticamente.

Pronto! Seu HTML está rodando localmente via servidor no VS Code.

## 🧪 Testes

``` bash
dotnet test
```

## 🛠️ Tecnologias utilizadas

-   C#
-   .NET 8
-   ASP.NET Core Web API
-   Swagger/OpenAPI

## Contribuição

1.  Faça um fork\
2.  Crie uma branch\
3.  Commit\
4.  Push\
5.  Abra um Pull Request

## Contato

GitHub: @gabrielwilliandev
