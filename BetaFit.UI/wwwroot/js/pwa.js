(() => {
 let installEvent;
 const buttons=document.querySelectorAll('[data-pwa-install]'),status=document.querySelector('[data-pwa-status]');
 const standalone=matchMedia('(display-mode: standalone)').matches||navigator.standalone;
 if(status)status.textContent=standalone?'Beta Fit já está aberto como aplicativo.':'Se o botão de instalação não aparecer, siga as instruções abaixo.';
 window.addEventListener('beforeinstallprompt',event=>{event.preventDefault();installEvent=event;buttons.forEach(b=>b.hidden=false);});
 buttons.forEach(button=>button.addEventListener('click',async()=>{if(!installEvent)return;await installEvent.prompt();const result=await installEvent.userChoice;installEvent=null;buttons.forEach(b=>b.hidden=true);if(status)status.textContent=result.outcome==='accepted'?'Instalação solicitada.':'Instalação cancelada. Você pode continuar no navegador.';}));
 window.addEventListener('appinstalled',()=>{buttons.forEach(b=>b.hidden=true);if(status)status.textContent='Beta Fit instalado!';});
 if('serviceWorker' in navigator&&window.isSecureContext)navigator.serviceWorker.register('/sw.js',{updateViaCache:'none'}).catch(()=>{if(status)status.textContent='Não foi possível preparar a instalação agora. Recarregue para tentar novamente.';});
})();
