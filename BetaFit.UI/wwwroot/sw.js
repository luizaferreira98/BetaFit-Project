const CACHE='betafit-offline-v4';
const SHELL=['/offline.html','/icons/app-192.png','/icons/app-512.png'];
self.addEventListener('install',event=>{event.waitUntil(caches.open(CACHE).then(cache=>cache.addAll(SHELL)));});
self.addEventListener('activate',event=>{event.waitUntil(caches.keys().then(keys=>Promise.all(keys.filter(key=>key.startsWith('betafit-')&&key!==CACHE).map(key=>caches.delete(key)))).then(()=>self.clients.claim()));});
// Never cache authenticated HTML, API responses, payment data or POST requests.
self.addEventListener('fetch',event=>{
 if(event.request.method!=='GET'||new URL(event.request.url).origin!==self.location.origin)return;
 if(event.request.mode==='navigate')event.respondWith(fetch(event.request).catch(()=>caches.match('/offline.html')));
});
