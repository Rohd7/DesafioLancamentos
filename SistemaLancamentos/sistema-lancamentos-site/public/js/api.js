const API = "http://localhost:5099";

async function postTransaction(payload) {
  await fetch(`${API}/transactions`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload)
  });
}

async function getTransactions() {
  const res = await fetch(`${API}/transactions`);
  return res.json();
}