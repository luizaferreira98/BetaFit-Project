(() => {
 const tip=document.createElement('div');tip.className='bf-ui-tooltip';tip.id='bf-ui-tooltip';tip.role='tooltip';tip.hidden=true;document.body.append(tip);
 const hide=()=>{tip.hidden=true;document.querySelectorAll('[aria-describedby="bf-ui-tooltip"]').forEach(el=>el.removeAttribute('aria-describedby'));};
 document.querySelectorAll('.bf-header #header-actions [data-tooltip]').forEach(el=>{
  const show=()=>{tip.textContent=el.getAttribute('aria-label')||el.dataset.tooltip;tip.hidden=false;el.setAttribute('aria-describedby',tip.id);const r=el.getBoundingClientRect();tip.style.left=Math.max(8,Math.min(innerWidth-tip.offsetWidth-8,r.left+r.width/2-tip.offsetWidth/2))+'px';tip.style.top=(r.bottom+8)+'px';};
  el.addEventListener('mouseenter',show);el.addEventListener('focus',show);el.addEventListener('mouseleave',hide);el.addEventListener('blur',hide);el.addEventListener('click',hide);
 });
 window.addEventListener('scroll',hide,true);window.addEventListener('resize',hide);document.addEventListener('keydown',e=>{if(e.key==='Escape')hide();});
 const bulk=document.querySelector('#review-bulk');if(bulk){const update=()=>{const n=document.querySelectorAll('[form="review-bulk"]:checked').length;const button=bulk.querySelector('button');button.disabled=n===0;button.textContent=n?`Aplicar a ${n} selecionada(s)`:'Selecione avaliações';};document.querySelectorAll('[form="review-bulk"],[data-review-all]').forEach(el=>el.addEventListener('change',update));update();}
})();
