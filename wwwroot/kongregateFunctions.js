window.kongregateFunctions = {

    getUsername: function () {
        // You can now access the Kongregate API with:
        // kongregate.services.getUsername(), etc
        // Proceed with loading your game...

        if (window.kongregate.services.isGuest()) {
            return "You are a guest.";
        } else {
            return window.kongregate.services.getUsername();
        }
    },
    updateTotalLevelScore: function (totalLevel) {
        window.kongregate.stats.submit("Total Level", totalLevel);
    },
    createSortableList: function (listElement) {
        Sortable.create(listElement, {});
    }
};