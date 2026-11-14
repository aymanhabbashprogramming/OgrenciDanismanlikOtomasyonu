(function () {
    'use strict';

    function initTableSort(table) {
        var headers = table.querySelectorAll('th[data-sort]');

        headers.forEach(function (th) {
            if (th.dataset.sortDir) {
                var icon = document.createElement('span');
                icon.className = 'sort-icon ms-1';
                icon.textContent = th.dataset.sortDir === 'asc' ? '↑' : '↓';
                th.appendChild(icon);
            }

            th.style.cursor     = 'pointer';
            th.style.userSelect = 'none';

            th.addEventListener('click', function () {
                var colIndex  = Array.from(th.parentElement.children).indexOf(th);
                var sortType  = th.dataset.sort;
                var ascending = th.dataset.sortDir !== 'asc';

                headers.forEach(function (h) {
                    if (h !== th) {
                        delete h.dataset.sortDir;
                        var existingIcon = h.querySelector('.sort-icon');
                        if (existingIcon) existingIcon.remove();
                    }
                });

                th.dataset.sortDir = ascending ? 'asc' : 'desc';

                var sortIcon = th.querySelector('.sort-icon');
                if (!sortIcon) {
                    sortIcon = document.createElement('span');
                    sortIcon.className = 'sort-icon ms-1';
                    th.appendChild(sortIcon);
                }
                sortIcon.textContent = ascending ? '↑' : '↓';

                var tbody = table.querySelector('tbody');
                var rows  = Array.from(tbody.querySelectorAll('tr'));

                rows.sort(function (a, b) {
                    var aCell = a.querySelectorAll('td')[colIndex];
                    var bCell = b.querySelectorAll('td')[colIndex];
                    var aVal  = aCell ? (aCell.dataset.sortValue !== undefined ? aCell.dataset.sortValue : aCell.textContent.trim()) : '';
                    var bVal  = bCell ? (bCell.dataset.sortValue !== undefined ? bCell.dataset.sortValue : bCell.textContent.trim()) : '';

                    if (sortType === 'num') {
                        return ascending
                            ? parseFloat(aVal) - parseFloat(bVal)
                            : parseFloat(bVal) - parseFloat(aVal);
                    }
                    return ascending
                        ? aVal.localeCompare(bVal, 'tr')
                        : bVal.localeCompare(aVal, 'tr');
                });

                rows.forEach(function (row) { tbody.appendChild(row); });
            });
        });
    }

    document.querySelectorAll('table[data-sortable]').forEach(initTableSort);
})();
