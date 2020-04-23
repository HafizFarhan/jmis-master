var map;
var view;
var centerPoint;

$(document).ready(function () {
    IntializeMap();
});

function IntializeMap() {
    getMap();
}


function setMapTarget(target) {
    map.setTarget(target);
}

function getMap() {
    if (map === null || map === undefined) {
        map = new ol.Map({
            controls: ol.control.defaults({ zoom: false, rotate: false }),
            interactions: ol.interaction.defaults({ doubleClickZoom: false }),
            view: getView()
        });
    }
    return map;
}

function getView(zoomLevel = 15, centerPointLat = 66.9742, centerPointLon = 24.8400) {
    if (view === null || view === undefined) {
        view = new ol.View({
            center: ol.proj.fromLonLat([centerPointLat, centerPointLon]),
            zoom: zoomLevel
        });
    }
    return view;
}

function getStrokeStyle(color, width) {
    return new ol.style.Stroke({
        color: color,
        width: width
    });
}

function getFillStyle(fillColor) {
    return new ol.style.Fill({
        color: fillColor
    }); 
}

function setCenter(lat, lon) {
    var center = ol.proj.transform([lat, lon], 'EPSG:4326', 'EPSG:3857');
}

