window.kongregateFunctions = {
    
    getUsername: function () {
        // You can now access the Kongregate API with:
        // kongregate.services.getUsername(), etc
        // Proceed with loading your game...

        if (window.kongregate.services.isGuest()) {
            return "";
        } else {
            return window.kongregate.services.getUsername();
        }
    },
	    getUserID: function () {
        // You can now access the Kongregate API with:
        // kongregate.services.getUsername(), etc
        // Proceed with loading your game...

        if (window.kongregate.services.isGuest()) {
            return "";
        } else {
            return window.kongregate.services.getUserId();
        }
    },
	getToken: function () {
        if (window.kongregate.services.isGuest()) {
            return "";
        }
        else {
            return window.kongregate.services.getGameAuthToken();
        }
    },
    showRegistration: function (dotNetInstance) {
        window.dotNet = dotNetInstance;
        window.kongregate.services.showRegistrationBox();
        window.kongregate.services.addEventListener("login", window.kongregateFunctions.onKongregatePageLogin);
    },
    onKongregatePageLogin: function () {
        window.dotNet.invokeMethodAsync('RefreshUI');
    },
    updateTotalLevelScore: function (totalLevel) {
        window.kongregate.stats.submit("Total Level", totalLevel);
    },
    updateTotalKills: function (totalKills) {
        window.kongregate.stats.submit("Total Kills", totalKills);
    },
    purchasePet: function (petIdentifier, dotNetInstance) {               

        if (kongregate.services.isGuest() == false) {
            window.dotNet = dotNetInstance;
            window.kongregate.mtx.purchaseItems([petIdentifier], window.kongregateFunctions.onPurchaseResult);
        }
        else {
            console.log("Is guest.");
        }
        
    },

    onPurchaseResult: function (result) {      
        if (result.success == true) {
            window.dotNet.invokeMethodAsync('PurchasePet');    
        }
        else {
            window.dotNet.invokeMethodAsync('CancelPurchase');     
        }
        
    },
    restorePurchases: function (dotNetInstance) {
        if (kongregate.services.isGuest() == false) {
            window.dotNet = dotNetInstance;
            window.kongregate.mtx.requestUserItemList("", window.kongregateFunctions.onRestorePurchasesResult);
        }
    },
    onRestorePurchasesResult: function (result) {
        if (result.success) {
            for (var i = 0; i < result.data.length; i++) {
                var item = result.data[i];
                window.dotNet.invokeMethodAsync('RestorePurchases', item.identifier);
            }      
        }
    },
    createSortableList: function (listElement) {
        Sortable.create(listElement, {
            group: "localStorage-example",
            easing: "cubic-bezier(0.075, 0.82, 0.165, 1)",
            animation: 90,
            sort:false,
            store: {
                /**
                 * Get the order of elements. Called once during initialization.
                 * @param   {Sortable}  sortable
                 * @returns {Array}
                 */
                get: function (sortable) {
                    var order = localStorage.getItem(sortable.options.group.name);
                    return order ? order.split('|') : [];
                },

                /**
                 * Save the order of elements. Called onEnd (when the item is dropped).
                 * @param {Sortable}  sortable
                 */
                set: function (sortable) {
                    var order = sortable.toArray();
                    localStorage.setItem(sortable.options.group.name, order.join('|'));
                }
            },

        })
    
    }
};