const app = document.getElementById("app");

function render() {
  const user = getUser();

  if (!user) return renderLogin(app);

  app.innerHTML = `
    <nav>
      <div>
        <button onclick="go('lanc')">Lançamentos</button>
        <button onclick="go('hist')">Histórico</button>
      </div>
      <div>
        ${user}
        <button onclick="logout()">Sair</button>
      </div>
    </nav>
    <div class="container" id="content"></div>
  `;

  go("lanc");
}

function go(page) {
  const content = document.getElementById("content");

  if (page === "lanc") renderLancamentos(content);
  if (page === "hist") renderHistorico(content);
}

function logout() {
  localStorage.removeItem("user");
  render();
}

render();