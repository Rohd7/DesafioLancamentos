function getUser() {
  return localStorage.getItem("user");
}

function login(user) {
  localStorage.setItem("user", user);
}