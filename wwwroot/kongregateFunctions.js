window.kongregateFunctions = {

    loadAPI:function() {
        window.kongregate = kongregateAPI.getAPI();
        // You can now access the Kongregate API with:
        // kongregate.services.getUsername(), etc
        // Proceed with loading your game...
        console.log(window.kongregate.services.getUsername());
    }

};