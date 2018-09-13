MiApp.LaddaButtonSubmit = function () {
    return {
        init: function () {
            Ladda.bind('div:not(.progress-demo) button'/*, { timeout: 2000 }*/);
        }
    }
}().init();