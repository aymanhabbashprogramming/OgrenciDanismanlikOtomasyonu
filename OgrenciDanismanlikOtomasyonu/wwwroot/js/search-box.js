(function () {
    function initSearchBox(wrapper) {
        var tableId   = wrapper.dataset.tableId;
        var label     = wrapper.dataset.countLabel || 'sonuç';
        var inputEl   = wrapper.querySelector('.search-box-input');
        var clearBtn  = wrapper.querySelector('.search-clear-btn');
        var countEl   = wrapper.querySelector('.search-count');
        var tableEl   = document.getElementById(tableId);

        if (!inputEl || !tableEl) return;

        var tbody       = tableEl.querySelector('tbody');
        var columnCount = tableEl.querySelectorAll('thead th').length;

        var noResultRow = document.createElement('tr');
        noResultRow.className = 'search-no-result';
        noResultRow.innerHTML = '<td colspan="' + columnCount + '" class="text-center text-muted py-4">' +
                                '<i class="bi bi-search me-1"></i>Sonuç bulunamadı</td>';
        noResultRow.style.display = 'none';
        tbody.appendChild(noResultRow);

        var allRows = Array.from(tbody.querySelectorAll('tr')).filter(function (row) {
            return !row.classList.contains('search-no-result');
        });

        function toLowerTurkish(text) { return text.toLocaleLowerCase('tr-TR'); }

        function filter() {
            var query   = toLowerTurkish(inputEl.value);
            var visible = 0;
            allRows.forEach(function (row) {
                var match = !query || toLowerTurkish(row.textContent).includes(query);
                row.style.display = match ? '' : 'none';
                if (match) visible++;
            });
            clearBtn.style.display    = inputEl.value ? '' : 'none';
            noResultRow.style.display = (query && visible === 0) ? '' : 'none';
            countEl.textContent       = query ? (visible + ' ' + label) : '';
        }

        inputEl.addEventListener('input', filter);
        clearBtn.addEventListener('click', function () {
            inputEl.value = '';
            filter();
            inputEl.focus();
        });
    }

    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('.search-box-wrapper[data-table-id]').forEach(initSearchBox);
    });
})();
