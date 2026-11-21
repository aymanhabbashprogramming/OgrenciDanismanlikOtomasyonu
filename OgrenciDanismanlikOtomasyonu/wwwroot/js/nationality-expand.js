(function () {
    'use strict';

    var btn      = document.getElementById('natExpandBtn');
    var panel    = document.getElementById('natCountryPanel');
    var search   = document.getElementById('natCountrySearch');
    var list     = document.getElementById('natCountryList');
    var noResult = document.getElementById('natNoResults');

    if (!btn || !panel) return;

    btn.addEventListener('click', function () {
        var isExpanded = this.getAttribute('aria-expanded') === 'true';
        this.setAttribute('aria-expanded', String(!isExpanded));
        panel.hidden = isExpanded;

        if (isExpanded) {
            if (search) {
                search.value = '';
                filterRows('');
            }
        } else {
            if (search) {
                setTimeout(function () { search.focus(); }, 50);
            }
        }
    });

    btn.addEventListener('keydown', function (e) {
        if (e.key === 'Enter' || e.key === ' ') {
            e.preventDefault();
            btn.click();
        }
    });

    if (search) {
        search.addEventListener('input', function () {
            filterRows(this.value);
        });
    }

    function filterRows(term) {
        if (!list || !noResult) return;
        var rows       = list.querySelectorAll('.nat-country-row');
        var normalized = term.toLocaleLowerCase('tr-TR');
        var visible    = 0;

        rows.forEach(function (row) {
            var name  = (row.getAttribute('data-name') || '').toLocaleLowerCase('tr-TR');
            var match = normalized.length === 0 || name.indexOf(normalized) !== -1;
            row.hidden = !match;
            if (match) visible++;
        });

        noResult.hidden = visible > 0 || normalized.length === 0;
    }
}());
