function renderLancamentos(container) {
  container.innerHTML = `
    <h3>💳 Novo Lançamento</h3>

    <div class="card">

      <div style="display:flex; flex-wrap:wrap; gap:10px; align-items:center;">

        <select id="type">
          <option value="CREDIT">Crédito</option>
          <option value="DEBIT">Débito</option>
        </select>

        <input id="amount" type="number" step="0.01" placeholder="Valor">

        <select id="category">
          <option value="RECEITA">Receita</option>
          <option value="ALIMENTACAO">Alimentação</option>
          <option value="TRANSPORTE">Transporte</option>
          <option value="LAZER">Lazer</option>
          <option value="OUTROS">Outros</option>
        </select>

        <input id="description" placeholder="Descrição">

        <select id="accountType">
          <option value="CORRENTE">Corrente</option>
          <option value="SALARIO">Salário</option>
          <option value="POUPANCA">Poupança</option>
        </select>

        <button id="btnEnviar" onclick="enviar()">
          🚀 Enviar
        </button>

      </div>

      <div id="loading" style="display:none; margin-top:10px;">⏳ Enviando...</div>
      <div id="msg" style="margin-top:10px;"></div>

      <!-- TOGGLE PAYLOAD -->
      <div style="margin-top:20px;">
        <button onclick="togglePayload()">
          📦 Ver Payload
        </button>

        <pre id="payloadPreview" style="
          display:none;
          background:#1e293b;
          color:#e2e8f0;
          padding:10px;
          border-radius:8px;
          margin-top:10px;
        "></pre>
      </div>

    </div>
  `;
}

function togglePayload() {
  const preview = document.getElementById("payloadPreview");

  preview.style.display =
    preview.style.display === "none" ? "block" : "none";
}

async function enviar() {
  const btn = document.getElementById("btnEnviar");
  const loading = document.getElementById("loading");
  const msg = document.getElementById("msg");
  const preview = document.getElementById("payloadPreview");

  msg.innerText = "";
  loading.style.display = "block";
  btn.disabled = true;

  const amountValue = parseFloat(document.getElementById("amount").value);

  if (!amountValue || amountValue <= 0) {
    loading.style.display = "none";
    btn.disabled = false;

    msg.innerText = "❌ Valor inválido";
    msg.style.color = "red";
    return;
  }

  const payload = {
    type: document.getElementById("type").value,
    amount: amountValue,
    currency: "BRL",
    description: document.getElementById("description").value,
    category: document.getElementById("category").value,
    transactionDate: new Date().toISOString().split("T")[0],

    account: {
      accountId: crypto.randomUUID(),
      accountType: document.getElementById("accountType").value,
      ownerId: crypto.randomUUID()
    },

    origin: {
      channel: "WEB",
      ip: "127.0.0.1",
      userAgent: navigator.userAgent
    },

    metadata: {
      tags: ["web"],
      notes: "envio manual frontend"
    }
  };

  // atualiza preview (mas escondido)
  preview.innerText = JSON.stringify(payload, null, 2);

  try {
    await postTransaction(payload);

    msg.innerText = "✅ Enviado com sucesso!";
    msg.style.color = "green";

    document.getElementById("amount").value = "";
    document.getElementById("description").value = "";
    document.getElementById("category").selectedIndex = 0;

  } catch (err) {
    console.error(err);
    msg.innerText = "❌ Erro ao enviar!";
    msg.style.color = "red";
  } finally {
    loading.style.display = "none";
    btn.disabled = false;
  }
}