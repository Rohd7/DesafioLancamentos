function renderHistorico(container) {
  container.innerHTML = `
    <h3>📊 Histórico</h3>

    <div class="card">

      <div style="display:flex; flex-wrap:wrap; gap:10px; align-items:center;">

        <label>Data Inicial</label>
        <input type="date" id="startDate">

        <label>Data Final</label>
        <input type="date" id="endDate">

        <select id="type">
          <option value="">Tipo</option>
          <option value="CREDIT">Crédito</option>
          <option value="DEBIT">Débito</option>
        </select>

        <select id="accountType">
          <option value="">Conta</option>
          <option value="CORRENTE">Corrente</option>
          <option value="SALARIO">Salário</option>
          <option value="POUPANCA">Poupança</option>
        </select>

        <button onclick="buscar()">
          🔍 Buscar
        </button>

      </div>

      <div id="loading" style="display:none; margin-top:10px;">
        ⏳ Carregando...
      </div>

      <div id="lista"></div>

    </div>
  `;
}

function toggleDetails(index) {
  const el = document.getElementById(`details-${index}`);
  const btn = document.getElementById(`btn-${index}`);

  const isOpen = el.style.display === "block";

  el.style.display = isOpen ? "none" : "block";
  btn.innerText = isOpen ? "🔽" : "🔼";
}

async function buscar() {
  const lista = document.getElementById("lista");
  const loading = document.getElementById("loading");

  loading.style.display = "block";
  lista.innerHTML = "";

  let url = "http://localhost:5099/transactions?";

  const startDate = document.getElementById("startDate").value;
  const endDate = document.getElementById("endDate").value;
  const type = document.getElementById("type").value;
  const accountType = document.getElementById("accountType").value;

  if (startDate) url += `startDate=${startDate}&`;
  if (endDate) url += `endDate=${endDate}&`;
  if (type) url += `type=${type}&`;
  if (accountType) url += `accountType=${accountType}&`;

  try {
    const res = await fetch(url);
    const data = await res.json();

    if (data.length === 0) {
      lista.innerHTML = "<p>Nenhuma transação encontrada</p>";
      return;
    }

    data.forEach((t, index) => {

      const isCredit = t.type === "CREDIT";

      lista.innerHTML += `
        <div class="card" style="margin-top:10px;">

          <div style="display:flex; justify-content:space-between; align-items:center;">

            <div>

              <span style="
                background:${isCredit ? "#d1fae5" : "#fee2e2"};
                color:${isCredit ? "#065f46" : "#991b1b"};
                padding:4px 8px;
                border-radius:6px;
                font-size:12px;
                font-weight:bold;
              ">
                ${t.type}
              </span>

              <div style="margin-top:5px; font-weight:600;">
                R$ ${t.amount}
              </div>

              <div style="color:#555;">
                ${t.category}
              </div>

              <div style="font-size:13px;">
                ${t.description || "-"}
              </div>

              <small style="color:#888;">
                ${new Date(t.transactionDate).toLocaleDateString()}
              </small>

            </div>

            <button id="btn-${index}" onclick="toggleDetails(${index})">
              🔽
            </button>

          </div>

          <div id="details-${index}" style="
            display:none;
            margin-top:15px;
            padding:10px;
            background:#f9fafb;
            border-radius:8px;
          ">

            <div style="font-size:13px; line-height:1.6;">

              <b>💰 Valor:</b> R$ ${t.amount} <br>
              <b>📄 Descrição:</b> ${t.description || "-"} <br>
              <b>📂 Categoria:</b> ${t.category} <br>
              <b>💱 Moeda:</b> ${t.currency || "-"} <br>

              <hr>

              <b>🏦 Conta</b><br>
              ID: ${t.account?.accountId} <br>
              Tipo: ${t.account?.accountType} <br>
              Owner: ${t.account?.ownerId} <br>

              <hr>

              <b>📅 Datas</b><br>
              Transação: ${new Date(t.transactionDate).toLocaleString()} <br>
              Criação: ${new Date(t.createdAt).toLocaleString()} <br>

            </div>

          </div>

        </div>
      `;
    });

  } catch (err) {
    console.error(err);
    lista.innerHTML = "<p>Erro ao buscar</p>";
  } finally {
    loading.style.display = "none";
  }
}