
//Función para activar o desactivar si al salir de una página (mediante navegación) se genera un evento que podemos controlar,
//en nuestro caso para avisar al usuario mediante una ventana modal de que hay cambios
window.setConfirmOnUnload = function (activar) {
    if (activar) {
        window.addEventListener("beforeunload", handleBeforeUnload);
    } else {
        window.removeEventListener("beforeunload", handleBeforeUnload);
    }
};


//Esta funcion se genera al cerra la pestaña o refrescar el navegador. Muestra un mensaje general en función del navegador
function handleBeforeUnload(event) {
    event.preventDefault();
    event.returnValue = "Tienes cambios sin guardar. ¿Seguro que quieres salir?"; //este mensaje no se ve. Muestra uno el propio navegador
    return event.returnValue;
}