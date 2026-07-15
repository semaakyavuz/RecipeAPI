const API = '/api/Recipes';

function esc(str) {
  return String(str ?? '')
    .replace(/&/g, '&amp;').replace(/</g, '&lt;')
    .replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}

// ─── Recipe List ──────────────────────────────────────────────

async function loadRecipes() {
  const res = await fetch(API);
  const recipes = res.ok ? await res.json() : [];
  renderList(recipes);
}

function renderList(recipes) {
  const el = document.getElementById('recipe-list');
  if (!recipes.length) {
    el.innerHTML = '<p class="empty">Henüz tarif yok. İlk tarifi ekleyin!</p>';
    return;
  }
  el.innerHTML = recipes.map(r => `
    <div class="recipe-card" onclick="openDetail(${r.id})">
      <h2>${esc(r.title)}</h2>
      <p class="meta">${r.baseServings} kişilik &middot; ${r.recipeIngredients?.length ?? 0} malzeme</p>
      ${r.instructions
        ? `<p class="preview">${esc(r.instructions).slice(0, 90)}${r.instructions.length > 90 ? '…' : ''}</p>`
        : ''}
    </div>
  `).join('');
}

function openDetail(id) {
  // genişletilecek
  console.log('openDetail', id);
}

// ─── Add Recipe Modal ─────────────────────────────────────────

let ingIndex = 0;

function addIngredientRow() {
  ingIndex++;
  const row = document.createElement('div');
  row.className = 'ing-row';
  row.innerHTML = `
    <input type="text"   placeholder="Malzeme adı"  name="ingName_${ingIndex}"   required>
    <input type="number" placeholder="Miktar"        name="ingAmount_${ingIndex}" min="0.01" step="any" required>
    <input type="text"   placeholder="Birim"         name="ingUnit_${ingIndex}"   required>
    <button type="button" class="btn-icon btn-remove" onclick="this.parentElement.remove()">✕</button>
  `;
  document.getElementById('ingredients-list').appendChild(row);
}

function openModal() {
  ingIndex = 0;
  document.getElementById('ingredients-list').innerHTML = '';
  document.getElementById('form-add').reset();
  addIngredientRow();
  document.getElementById('modal-overlay').classList.remove('hidden');
  document.body.style.overflow = 'hidden';
}

function closeModal() {
  document.getElementById('modal-overlay').classList.add('hidden');
  document.body.style.overflow = '';
}

document.getElementById('btn-add').addEventListener('click', openModal);
document.getElementById('btn-close-modal').addEventListener('click', closeModal);
document.getElementById('btn-cancel').addEventListener('click', closeModal);
document.getElementById('btn-add-ingredient').addEventListener('click', addIngredientRow);
document.getElementById('modal-overlay').addEventListener('click', e => {
  if (e.target === e.currentTarget) closeModal();
});

document.getElementById('form-add').addEventListener('submit', async function (e) {
  e.preventDefault();
  const fd = new FormData(this);
  const body = {
    title: fd.get('title'),
    instructions: fd.get('instructions'),
    baseServings: parseInt(fd.get('baseServings'), 10),
    recipeIngredients: []
  };
  for (let i = 1; i <= ingIndex; i++) {
    const name = fd.get(`ingName_${i}`);
    if (!name) continue;
    body.recipeIngredients.push({
      amount: parseFloat(fd.get(`ingAmount_${i}`)),
      unit: fd.get(`ingUnit_${i}`),
      ingredient: { name, caloriesPerUnit: 0 }
    });
  }
  const res = await fetch(API, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body)
  });
  if (res.ok) { closeModal(); await loadRecipes(); }
  else alert('Tarif kaydedilemedi.');
});

loadRecipes();
