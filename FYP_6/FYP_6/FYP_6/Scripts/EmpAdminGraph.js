/// <reference path="loader.js" />

google.charts.load('current', { 'packages': ['corechart'] });
google.charts.setOnLoadCallback(drawVisualization);

function drawVisualization() {
    // Some raw data (not necessarily accurate)
    var Chartdata = google.visualization.arrayToDataTable(data);

    var options = {
        title: 'Yearly Statistics',
        vAxis: { title: 'Statistics' },
        hAxis: { title: 'Year' },
        seriesType: 'bars',
        series: { 5: { type: 'line' } }
    };

    var chart = new google.visualization.ComboChart(document.getElementById('chart_div'));
    chart.draw(Chartdata, options);
}
