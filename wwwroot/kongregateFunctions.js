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
    updateTotalLevelScore: function (totalLevel) {
        window.kongregate.stats.submit("Total Level", totalLevel);
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