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

document.getElementById('btn-add').addEventListener('click', () => {
  // genişletilecek
  alert('Form yakında eklenecek!');
});

loadRecipes();
