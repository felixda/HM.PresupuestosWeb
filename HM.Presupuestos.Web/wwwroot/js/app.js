
var _timerIdleTimeout;
var _timerInterval = 0;
var _blazorReady = false;
var _blazorReadyCallbacks = [];

// Marcar Blazor como listo y ejecutar callbacks pendientes
window.onBlazorReady = function() {
    _blazorReady = true;
    console.log('[Blazor] Runtime inicializado correctamente');
    
    // Ejecutar todos los callbacks pendientes
    while (_blazorReadyCallbacks.length > 0) {
        var callback = _blazorReadyCallbacks.shift();
        try {
            callback();
        } catch (error) {
            console.error('[Blazor] Error ejecutando callback pendiente:', error);
        }
    }
};

// Función helper para verificar si Blazor está listo
window.JSRuntimeLoaded = () => {
    return _blazorReady && typeof DotNet !== 'undefined';
};

// Ejecutar función cuando Blazor esté listo, o encolarla si no lo está
window.whenBlazorReady = function(callback) {
    if (_blazorReady) {
        callback();
    } else {
        _blazorReadyCallbacks.push(callback);
    }
};

// Wrappers seguros para iniciar/finalizar el control de inactividad
window.startInactividadWhenReady = function(helper, inactividadMs, advertenciaMs) {
    window.whenBlazorReady(function() {
        if (window.inactividad && typeof window.inactividad.iniciar === 'function') {
            try {
                window.inactividad.iniciar(helper, inactividadMs, advertenciaMs);
            } catch (err) {
                console.error('[inactividad] Error al llamar a iniciar:', err);
            }
        } else {
            console.error('[inactividad] iniciar no disponible');
        }
    });
};

window.finalizarInactividadWhenReady = function(helper) {
    window.whenBlazorReady(function() {
        if (window.inactividad && typeof window.inactividad.finalizar === 'function') {
            try {
                window.inactividad.finalizar(helper);
            } catch (err) {
                console.error('[inactividad] Error al llamar a finalizar:', err);
            }
        } else {
            console.warn('[inactividad] finalizar no disponible');
        }
    });
};

// Generic invoker that calls a global function if it exists. Returns true if called, false otherwise.
window.invokeIfExists = function(functionName) {
    try {
        var func = window[functionName];
        if (typeof func === 'function') {
            // Build args excluding functionName
            var args = Array.prototype.slice.call(arguments, 1);
            func.apply(null, args);
            return true;
        }
        return false;
    } catch (err) {
        console.error('[invokeIfExists] Error invoking ' + functionName + ':', err);
        return false;
    }
};
// GetResourceValue — obtiene textos localizados via API REST (sin interop estático)
window.GetResourceValue = function (expression) {
    var idioma = (Cookie.Read('app_idioma') || 'es').trim();
    var url = '/api/recursos/' + encodeURIComponent(expression) + '/' + encodeURIComponent(idioma);
    return fetch(url)
        .then(function (r) { return r.ok ? r.json() : Promise.reject('HTTP ' + r.status); });
};





window.localStorageInterop = {
    saveItem: function (key, value) {
        // Guardar como string simple (sin JSON.stringify)
        localStorage.setItem(key, value);
    },
    getItem: function (key) {
        try {
            const item = localStorage.getItem(key);
            return item || null; // Devolver el string tal cual
        }
        catch(error) {
            console.error(`Error getting localStorage item '${key}':`, error);
            return null;
        } 
    },
    removeItem: function (key) {
        localStorage.removeItem(key);
    },
    clearStorage: function () {
        localStorage.clear();
    }
};



window.OpenNewTab = (args) => {
    window.open(args);
};



window.Page = {
    SetMenuFavorite: function (iconFavoriteId, isFavorite, classSelected, classUnselected) {
        var icon = document.getElementById(iconFavoriteId);
        if (icon) {
            if (isFavorite) {
                if (icon.classList.contains(classUnselected))
                    icon.classList.remove(classUnselected);
                icon.classList.add(classSelected);
            }
            else {
                if (icon.classList.contains(classSelected))
                    icon.classList.remove(classSelected);
                icon.classList.add(classUnselected);
            }
        }
    },
    SetPageTitle: function (suffixTitle) {
        const maxAttempts = 20;
        const intervalTime = 500; 
        let attempts = 0;
        let intervalId;
        intervalId = setInterval(() => {
            attempts++;
            var currentPageTitle = document.title;
            
            // Sucess: currentPageTitle not empty -> stop
            if (currentPageTitle && !currentPageTitle.endsWith(suffixTitle)) {
                document.title = currentPageTitle + ' - ' + suffixTitle;
                //console.log(`[Sucess not empty - Attempt ${attempts}] Title updated: ${document.title}`);
                clearInterval(intervalId);
            }
            // Sucess: title filled -> stop
            else if (currentPageTitle.endsWith(suffixTitle))
            {
                //console.log(`[Sucess filled - Attempt ${attempts}] Title updated: ${document.title}`);
                clearInterval(intervalId);
            }
            //Fail: Max attemps -> stop
            else if (attempts >= maxAttempts) {
                //console.warn(`[Fail] Max attempts ${maxAttempts}`);
                clearInterval(intervalId);
            }
            //Title fail  -> try
            else {
                //console.log(`[Attempt ${attempts}/${maxAttempts}] current title: "${currentPageTitle}".`);
            }
        }, intervalTime); 
    }
};

window.Menu = {
    ToggleMenuVisibility: function () {
        var menuContainer = document.getElementById('menuContainer');
        if (menuContainer.classList.contains('hidden'))
            menuContainer.classList.remove('hidden');
        else
            menuContainer.classList.add('hidden');
    },
    SetMenuActive: function (menuCode) {
        //se mete en un Timeout para esperar a que el menu este pintado y poder tener acceso a dicho menu
        setTimeout(() => {
            document.querySelectorAll('.menu-item_active').forEach(item => {
                item.classList.remove('menu-item_active');
            });
            var menuActive = document.getElementById('menu_' + menuCode);

            if (menuActive) {
                menuActive.classList.add('menu-item_active');
            }
        }, 100);
    }
};


window.Cookie = {
    Read: function (cookie_name) {
        //alert('Leyendo cookie');
        var name = cookie_name + "=";
        var decodedCookie = decodeURIComponent(document.cookie);
        var ca = decodedCookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                //alert('Leyendo cookie ->' + c.substring(name.length, c.length));
                return c.substring(name.length, c.length);
            }
        }
        //alert('Leyendo cookie -> null');
        return "";
    },
    Delete: function (cookie_name) {
        document.cookie = cookie_name + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
    },
    Write: function (cookieName, content, minutes) {
        let currentDate = new Date();
        let newDateExpires = new Date(currentDate.getTime() + (minutes * 60 * 1000));
        document.cookie = cookieName + "=" + content + ";expires=" + newDateExpires.toUTCString() + "; path=/";
    }
};



//Build timer to manage user inactivity
window.ResetIdleTimeout = function (interval) {
    if (interval > 0) {
        clearTimeout(_timerIdleTimeout);
        _timerIdleTimeout = setTimeout(() => {
            if (JSRuntimeLoaded()) {
                DotNet.invokeMethodAsync('HM.Presupuestos.Server', 'KeepAlive');
            }
           // Page.SetFlagChanges(false);
            window.location.href = '/';
        }, interval);
    }
};



window.AlertMessage = function (message) {
    alert(message);
};


window.selectInputTextByEvent = function () {
    const active = document.activeElement;
    
    if (active && active.tagName.toUpperCase() === 'INPUT') {
        active.select();
    }
};


//window.cursorHelper = {
//    setWait: function () {
//        document.body.classList.add("cursor-wait");
//    },
//    setDefault: function () {
//        document.body.classList.remove("cursor-wait");
//    }
//};

function validarDecimales(input, maxDecimales) {
    const valor = input.value;
    const partes = valor.split(',');

    // Si hay parte decimal y tiene más de los permitidos
    if (partes.length > 1 && partes[1].length > maxDecimales) {
        // Truncamos el exceso de decimales
        input.value = partes[0] + ',' + partes[1].substring(0, maxDecimales);
    }
}




