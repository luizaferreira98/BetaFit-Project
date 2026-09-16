(() => {
    const form=document.querySelector('[data-password-change]');
    if(form){
        const password=form.elements.newPassword, confirmation=form.elements.confirmNewPassword;
        const submit=form.querySelector('button:not([type="button"])');
        const bar=form.querySelector('[data-strength-bar]'), label=form.querySelector('[data-strength-label]');
        const check=()=>({length:password.value.length>=6,upper:/[A-Z]/.test(password.value),lower:/[a-z]/.test(password.value),digit:/[0-9]/.test(password.value),special:/[^a-zA-Z0-9]/.test(password.value),match:password.value.length>0&&password.value===confirmation.value});
        const update=()=>{
            const rules=check(), score=Object.values(rules).filter(Boolean).length;
            form.querySelectorAll('[data-check]').forEach(item=>item.classList.toggle('is-met',rules[item.dataset.check]));
            bar.style.width=(score/6*100)+'%';bar.style.background=score===6?'#72a221':score>2?'#b77c1c':'#b85454';
            label.textContent=!password.value?'Digite sua nova senha.':score===6?'Senha pronta. Você pode continuar.':'Complete os requisitos abaixo para continuar.';
            submit.disabled=!Object.values(rules).every(Boolean)||!form.elements.currentPassword.value;
            confirmation.setCustomValidity(confirmation.value&& !rules.match?'As senhas não coincidem.':'');
        };
        form.addEventListener('input',update);form.addEventListener('submit',e=>{update();if(submit.disabled)e.preventDefault();});update();
    }
    const conversation=document.querySelector('[data-store-conversation]');
    if(!conversation)return;
    const log=conversation.querySelector('[data-store-log]'), status=conversation.querySelector('[data-store-status]');
    const send=conversation.querySelector('[data-store-send]'), button=send.querySelector('button[type="submit"]');
    let stopped=false,refreshing=false;
    const seen=new Set([...log.querySelectorAll('[data-message-id]')].map(e=>Number(e.dataset.messageId)));
    log.scrollTop=log.scrollHeight;
    const refresh=async()=>{
        if(stopped||document.hidden||refreshing)return;
        refreshing=true;
        try{
            const response=await fetch(conversation.dataset.threadUrl,{headers:{Accept:'application/json'},cache:'no-store'});
            if(!response.ok||response.redirected)throw new Error();
            const data=await response.json();const nearBottom=log.scrollHeight-log.scrollTop-log.clientHeight<80;
            for(const m of data.messages){
                if(seen.has(m.id))continue;seen.add(m.id);
                const p=document.createElement('p');p.className='bf-chat-message'+(m.isStaff?'':' is-user');p.dataset.messageId=m.id;
                const name=document.createElement('strong');name.textContent=(m.isStaff?'Equipe BetaFit':'Cliente')+' · '+new Date(m.createdAt.endsWith('Z')?m.createdAt:m.createdAt+'Z').toLocaleString('pt-BR');
                p.append(name,document.createTextNode(m.text));log.append(p);
            }
            if(nearBottom)log.scrollTop=log.scrollHeight;
            status.textContent='Conversa atualizada. A equipe responderá por aqui.';
        }catch{status.textContent='Não foi possível atualizar a conversa. Tentaremos novamente em alguns segundos.';}
        finally{refreshing=false;}
    };
    send.addEventListener('submit',async event=>{
        event.preventDefault();if(button.disabled)return;
        button.disabled=true;status.textContent='Enviando sua mensagem…';
        try{
            const response=await fetch(send.action,{method:'POST',body:new FormData(send),headers:{Accept:'application/json'}});
            if(!response.ok||response.redirected)throw new Error();
            send.elements.Text.value='';await refresh();log.scrollTop=log.scrollHeight;
        }catch{status.textContent='Não foi possível confirmar o envio. Atualize a conversa antes de tentar novamente; seu texto foi mantido.';}
        finally{button.disabled=false;}
    });
    let timer=setInterval(refresh,5000);document.addEventListener('visibilitychange',()=>{if(!document.hidden)refresh();});
    window.addEventListener('pagehide',()=>{stopped=true;clearInterval(timer);});
    window.addEventListener('pageshow',event=>{if(event.persisted){stopped=false;timer=setInterval(refresh,5000);refresh();}});
    refresh();
})();
