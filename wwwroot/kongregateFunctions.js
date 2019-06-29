window.kongregateFunctions = {

    getUsername:function() {
        // You can now access the Kongregate API with:
        // kongregate.services.getUsername(), etc
        // Proceed with loading your game...
        if (kongregate.services.isGuest()) {
            return "You are a guest.";
        } else {
            return kongregate.getUsername();
        }
    },
    updateTotalLevelScore: function (totalLevel) {
        kongregate.stats.submit("Total Level", totalLevel);
    },
    createSortableList: function (listElement) {
        Sortable.create(listElement, {});
    }
};