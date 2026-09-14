(() => {
  'use strict';
  const dialog = document.createElement('dialog');
  dialog.className = 'bf-dialog';
  dialog.setAttribute('aria-labelledby', 'bf-dialog-title');
  dialog.innerHTML = '<h2 id="bf-dialog-title">Confirme sua ação</h2><p data-message></p><div class="bf-form-actions"><button type="button" class="bf-btn bf-btn--ghost" data-cancel>Voltar</button><button type="button" class="bf-btn bf-btn--dark" data-ok>Confirmar</button></div>';
  document.body.append(dialog);
  const queue = []; let active = null;
  function next() {
    if (active || !queue.length) return;
    active = queue.shift();
    dialog.querySelector('[data-message]').textContent = active.message;
    dialog.querySelector('h2').textContent = active.confirm ? 'Confirme sua ação' : 'Aviso';
    dialog.querySelector('[data-cancel]').hidden = !active.confirm;
    dialog.querySelector('[data-ok]').textContent = active.confirm ? 'Confirmar' : 'Entendi';
    dialog.showModal();
    dialog.querySelector(active.confirm ? '[data-cancel]' : '[data-ok]').focus();
  }
  function finish(ok) { const previous = active; active = null; dialog.close(); previous.resolve(ok); previous.focus?.focus(); next(); }
  function ask(message, confirm = false) { return new Promise(resolve => { queue.push({message,confirm,resolve,focus:document.activeElement}); next(); }); }
  window.bfNotice = message => ask(message);
  dialog.querySelector('[data-cancel]').onclick = () => finish(false);
  dialog.querySelector('[data-ok]').onclick = () => finish(true);
  dialog.addEventListener('cancel', e => { e.preventDefault(); finish(false); });
  document.addEventListener('submit', async e => {
    const form = e.target, button = e.submitter;
    const message = button?.dataset.confirm || form.dataset.confirm;
    if (!message || form.dataset.confirmed === 'true') { delete form.dataset.confirmed; return; }
    e.preventDefault();
    if (await ask(message, true)) { form.dataset.confirmed = 'true'; form.requestSubmit(button || undefined); }
  });
  document.querySelectorAll('.bf-header [aria-label],.bf-header [title]').forEach(el => {
    if (!el.matches('a,button')) return;
    el.dataset.tooltip = el.getAttribute('title') || el.getAttribute('aria-label');
    el.removeAttribute('title');
    el.addEventListener('mouseenter', () => { if (el.hasAttribute('aria-label')) el.dataset.tooltip = el.getAttribute('aria-label'); });
  });
  document.querySelector('[data-copy-pix]')?.addEventListener('click', async () => {
    const text = document.querySelector('#pix-code'), status = document.querySelector('[data-copy-status]');
    try { await navigator.clipboard.writeText(text.value); status.textContent = 'Código de exemplo copiado!'; }
    catch { text.focus(); text.select(); status.textContent = 'Selecione e copie o código com Ctrl+C ou o menu do celular.'; }
  });
  document.querySelector('[data-review-all]')?.addEventListener('change', e => {
    document.querySelectorAll('[form="review-bulk"][name="selectedIds"]').forEach(x => x.checked = e.target.checked);
  });
})();

(() => {
 const help=document.querySelector('#bf-help-panel'), trigger=document.querySelector('[data-help-open]');
 trigger?.addEventListener('click',()=>{help.showModal();trigger.setAttribute('aria-expanded','true');});
 document.querySelector('[data-help-close]')?.addEventListener('click',()=>help.close());
 help?.addEventListener('close',()=>{trigger.setAttribute('aria-expanded','false');trigger.focus();});
 help?.addEventListener('click',e=>{if(e.target===help){const r=help.getBoundingClientRect();if(e.clientX<r.left||e.clientX>r.right||e.clientY<r.top||e.clientY>r.bottom)help.close();}});
 document.querySelectorAll('[data-star-picker]').forEach(picker=>{
  const choices=[...picker.querySelectorAll('input')], output=picker.querySelector('output');
  const paint=n=>choices.forEach((input,i)=>input.closest('.bf-half-option').classList.toggle('is-filled',i<n));
  const chosen=()=>choices.findIndex(x=>x.checked)+1;
  choices.forEach((input,i)=>{input.addEventListener('change',()=>{paint(chosen());output.textContent=((i+1)/2).toLocaleString('pt-BR')+' de 5 estrelas';});input.addEventListener('focus',()=>paint(i+1));input.closest('.bf-half-option').addEventListener('pointerenter',()=>paint(i+1));});
  picker.addEventListener('pointerleave',()=>paint(chosen()));picker.addEventListener('focusout',()=>paint(chosen()));
 });
})();
