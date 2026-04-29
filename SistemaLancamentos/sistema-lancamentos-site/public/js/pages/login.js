function renderLogin(app) {
  app.innerHTML = `
    <div class="container">
      <h2>Login</h2>
      <input id="user" placeholder="Usuário">
      <button onclick="doLogin()">Entrar</button>
    </div>
  `;
}

function doLogin() {
  const user = document.getElementById("user").value;
  login(user);
  render();
}