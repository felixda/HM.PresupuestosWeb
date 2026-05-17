
//Creamos timer para controlar la inactividad del usuario en la pantalla
//controlando eventos de raton y teclado
//Un primer evento se genera para mostrar un aviso de que se va a volver a la pantalla Home
//Un segundo evento que nos lleva a la pagina inicial
//Y un tercer evento para actulizar la cuenta atras

window.inactividad = (() => {
    let timeoutInactividad = 5 * 60 * 1000; // 5 minutos
    let tiempoAdvertencia = 60 * 1000; // 1 minuto
    let intervaloCuenta = 1000; // cada segundo

    let dotNetHelper = null;
    let timeoutMain;
    let cuentaRegresivaInterval;
    let tiempoRestante;


    /// <summary>
    /// Función para inicialiar los contadores y suscribirse a los eventos del teclado y del raton
    /// </summary>
    /// <param name="helper">Referencia al objeto desde donde se llama a la funcion</param>
    /// <param name="inactividadMs">Tiempo en milisegundos para controlar la inactividad</param>
    /// <param name="advertenciaMs">Tiempo en milisegundos antes de volver a la home para mostrar una advertencia</param>
    function iniciar(helper, inactividadMs, advertenciaMs) {
        dotNetHelper = helper;
        timeoutInactividad = inactividadMs;
        tiempoAdvertencia = advertenciaMs;
        reiniciarTemporizador();

        //Controlamos movimientos de raton o teclados
        ['mousemove', 'keydown'].forEach(evt =>
            window.addEventListener(evt, detectarActividad)
        );
    }

    /// <summary>
    /// Función que detecta si hay actividad. Si está la ventana de aviso visible se cierra y se reinician los contadores
    /// </summary>
    function detectarActividad() {
        if (cuentaRegresivaInterval) {
            limpiarCuentaRegresiva();
            dotNetHelper.invokeMethodAsync('CancelarAdvertencia');
        }
        reiniciarTemporizador();
    }

    /// <summary>
    /// Función para inicialiar los contadores con los tiempos pasados
    /// </summary>
    function reiniciarTemporizador() {
        clearTimeout(timeoutMain);
        timeoutMain = setTimeout(mostrarAdvertencia, timeoutInactividad - tiempoAdvertencia);
    }

    /// <summary>
    /// Función para mostrar una ventana de aviso indicando que después del tiempo de cuenta atras, nos redirigira a la pagina de Home
    /// Si mientras esta visible esta pantalla hay algun evento de raton o teclado, se cierra y se reinician los contadores
    /// Si la cuenta atras llega a su fin, manda un evento al servicio para indicar a la pantalla que deber ir a la Home
    /// </summary>
    function mostrarAdvertencia() {
        dotNetHelper.invokeMethodAsync('InactividadIniciada');
        tiempoRestante = tiempoAdvertencia;

        cuentaRegresivaInterval = setInterval(() => {
            tiempoRestante -= intervaloCuenta;
            //Para actualizar en la ventana de aviso la cuenta atras
            dotNetHelper.invokeMethodAsync('CuentaRegresiva', tiempoRestante);

            //Si el tiempo ha finalizado, mandamos evento indicandolo
            if (tiempoRestante <= 0) {
                limpiarCuentaRegresiva();
                dotNetHelper.invokeMethodAsync('InactividadFinalizada');
            }
        }, intervaloCuenta);
    }

    function limpiarCuentaRegresiva() {
        clearInterval(cuentaRegresivaInterval);
        cuentaRegresivaInterval = null;
    }

    /// <summary>
    /// Función para cancelar la suscripción a los eventos del teclado y raton
    /// </summary>
    function finalizar() {
        console.log("Desuscripción de eventos de inactividad.");
        ['mousemove', 'keydown'].forEach(evt =>
            window.removeEventListener(evt, detectarActividad)
        );
    }

    return {
        iniciar,
        finalizar
    };
})();


