//#region OpenLayers

var pakShipsArray = { 463043101: "SIBI", 463047101: "SHALAMAR", 463036101: "LAHORE", 463037101: "CHITRAL", 463034101: "QUETTA", 463042101: "HYDERABAD", 463035101: "KARACHI", 463046101: "MULTAN", 463041101: "MALAKAND", 463065101: "KHAIRPUR", 463064101: "BOLAN" };

document.addEventListener('DOMContentLoaded', function (e) {
    // Start the connection.
    var connection = new signalR.HubConnectionBuilder()
        .withUrl('pushmessages')
        .build();
    // Transport fallback functionality is now built into start.
    connection.start()
        .then(function () {
            console.log('connection started');
        })
        .catch(error => {
            console.error(error.message);
        });
    // Create a function that the hub can call to broadcast messages.
    connection.on('PushAISTrack', function (track) {
        if (track.tracK_NUMBER && track.lat && track.lon) {
            if (track.lat >= -90 && track.lat <= 90 && track.lon >= -180 && track.lon <= 180) {
                var trackProps = {
                    geometry: new ol.geom.Point(ol.proj.fromLonLat([track.lon, track.lat])),
                    type: 'Track',
                    trackNumber: track.tracK_NUMBER,
                    imo: track.imo,
                    trackType: track.tracktype,
                    lat: track.lat,
                    lon: track.lon,
                    course: track.course,
                    speed: track.speed,
                    IsLloydInfoPresent: track.isLloydInfoPresent,
                    LlydDetail: track.llydDetail
                };

                if (track.speed === 0) {
                    var Opacity = 0.5;
                }
                else {
                    Opacity = 1;
                }

                var trackMarkerStyle = switchMarker(track.shiptypename, track.course, Opacity);

                if (typeof pakShipsArray[track.tracK_NUMBER] !== "undefined") {
                    console.log("Pakistani SHIP Name: " + pakShipsArray[track.tracK_NUMBER])
                    trackMarkerStyle.setText(pakShipsArray[track.tracK_NUMBER]);
                }

                var localAISFeature = localAISVectorSource.getFeatureById(track.tracK_NUMBER);
                if (localAISFeature === null) {
                    var trackMarker = new ol.Feature(trackProps);
                    localAISVectorSource.addFeature(trackMarker);
                    trackMarker.setStyle(trackMarkerStyle);
                    trackMarker.setId(track.tracK_NUMBER);
                } else {
                    localAISFeature.setProperties(trackProps);
                    localAISFeature.setStyle(trackMarkerStyle);
                }
                //switch (track.tracktype) {
                //    case "Local AIS":
                //        var localAISFeature = localAISVectorSource.getFeatureById(track.tracK_NUMBER);
                //        if (localAISFeature === null) {
                //            var trackMarker = new ol.Feature(trackProps);
                //            localAISVectorSource.addFeature(trackMarker);
                //            trackMarker.setStyle(trackMarkerStyle);
                //            trackMarker.setId(track.tracK_NUMBER);
                //        } else {
                //            localAISFeature.setProperties(trackProps);
                //            localAISFeature.setStyle(trackMarkerStyle);
                //            localAISFeature.setId(track.tracK_NUMBER);
                //        }
                //        break;
                //    case 'CSN AIS':
                //        var csnAISFeature = csnAISVectorSource.getFeatureById(track.tracK_NUMBER);
                //        if (csnAISFeature === null) {
                //            trackMarker = new ol.Feature(trackProps);
                //            csnAISVectorSource.addFeature(trackMarker);
                //        } else {
                //            csnAISFeature.setProperties(trackProps);
                //            csnAISFeature.setStyle(trackMarkerStyle);
                //            csnAISFeature.setId(track.tracK_NUMBER);
                //        }
                //        break;
                //    case 'CSN Raddar':
                //        var furunoFeature = furunoVectorSource.getFeatureById(track.tracK_NUMBER);
                //        if (furunoFeature === null) {
                //            trackMarker = new ol.Feature(trackProps);
                //            furunoVectorSource.addFeature(trackMarker);
                //        } else {
                //            furunoFeature.setProperties(trackProps);
                //            furunoFeature.setStyle(trackMarkerStyle);
                //            furunoFeature.setId(track.tracK_NUMBER);
                //        }
                //        break;
                //}                                          
            }
        }
        //addToTrackList(track.tracK_NUMBER, track.lat, track.lon, track.speed, track.course, track.tracktype);
    });
    connection.on('PushAISTrackUpdate', function (track) {

        var localAISFeature = localAISVectorSource.getFeatureById(track.tracK_NUMBER);

        if (localAISFeature !== null) {
            localAISFeature.setProperties({
                destination: track.destination,
                shipName: track.tracK_LABEL
            });

            var speed = localAISFeature.get('speed');
            var course = localAISFeature.get('course');

            if (speed === 0) {
                var Opacity = 0.5;
            }
            else {
                Opacity = 1;
            }

            var trackMarkerStyle = switchMarker(track.shiptypename, course, Opacity);
            localAISFeature.setStyle(trackMarkerStyle);
        }
    });
    connection.on('PushCOIs', function (COI) {
        if ((COI.latitude >= -90 && COI.latitude <= 90) && (COI.longitude >= -180 && COI.longitude <= 180)) {
            $(document).ready(function () {
                coiMarker = new ol.Feature({
                    geometry: new ol.geom.Point(ol.proj.fromLonLat([COI.longitude, COI.latitude])),
                    type: 'COIMarker',
                    reportingDatetime: COI.reportingDatetime,
                    coiNumber: COI.coiNumber,
                    latitude: COI.latitude,
                    longitude: COI.longitude,
                    coiTypeName: COI.coiTypeName
                });
                coiMarker.setId(COI.coiNumber);
                coiVectorSource.addFeature(coiMarker);
            });
        }
    });
    connection.on('PushLR', function (lostReport) {
        if ((lostReport.latitude >= -90 && lostReport.latitude <= 90) && (lostReport.longitude >= -180 && lostReport.longitude <= 180)) {
            LRMarker = new ol.Feature({
                geometry: new ol.geom.Point(ol.proj.fromLonLat([lostReport.longitude, lostReport.latitude])),
                type: 'LRMarker',
                reportingDatetime: lostReport.reportingDatetime,
                latitude: lostReport.latitude,
                longitude: lostReport.longitude
            });
            LRMarker.setId(lostReport.id);
            LRVectorSource.addFeature(LRMarker);
        }
    });
    connection.on('PushDR', function (dropReport) {
        plotDRMarker(dropReport);
    });
    connection.on('PushNews', function (newsViewModel) {
        console.log(newsViewModel);
    });
    connection.on('PushSR', function (subsequentReport) {
        if (subsequentReport.coiNumber === null) {
            if ((subsequentReport.latitude >= -90 && subsequentReport.latitude <= 90) && (subsequentReport.longitude >= -180 && subsequentReport.longitude <= 180)) {
                srMarker = new ol.Feature({
                    geometry: new ol.geom.Point(ol.proj.fromLonLat([subsequentReport.longitude, subsequentReport.latitude])),
                    type: 'SRMarker',
                    reportingDatetime: subsequentReport.reportingDatetime,
                    srNumber: subsequentReport.srNumber,
                    prNumber: subsequentReport.prNumber,
                    latitude: subsequentReport.latitude,
                    longitude: subsequentReport.longitude,
                    coiTypeName: subsequentReport.coiTypeName
                });
                srMarker.setId(subsequentReport.srNumber);
                srVectorSource.addFeature(srMarker);
            }
        }
        else {
            var srFeature = srVectorSource.getFeatureById(subsequentReport.srNumber);
            srVectorSource.removeFeature(srFeature);
        }
    });
    connection.on('PushAR', function (amplifyingReport) {
        if ((amplifyingReport.latitude >= -90 && amplifyingReport.latitude <= 90) && (amplifyingReport.longitude >= -180 && amplifyingReport.longitude <= 180)) {
            ARMarker = new ol.Feature({
                geometry: new ol.geom.Point(ol.proj.fromLonLat([amplifyingReport.longitude, amplifyingReport.latitude])),
                type: 'ARMarker',
                reportingDatetime: amplifyingReport.reportingDatetime,
                arNumber: amplifyingReport.arNumber,
                latitude: amplifyingReport.latitude,
                longitude: amplifyingReport.longitude,
                coiClassificationName: amplifyingReport.coiClassificationName
            });
            ARMarker.setId(amplifyingReport.arNumber);
            ARVectorSource.addFeature(ARMarker);
        }
    });
    connection.on('PushAAR', function (afterActionReport) {
    });
    connection.on('PushPR', function (preliminaryReport) {
        if (preliminaryReport.coiNumber === null) {
            plotPRMarker(preliminaryReport);
        }
        else {
            var prFeature = prVectorSource.getFeatureById(preliminaryReport.prNumber);
            prVectorSource.removeFeature(prFeature);
        }
    });
    connection.on('DeletePR', function (prNumber) {
        if (prNumber !== null) {
            var prFeature = prVectorSource.getFeatureById(prNumber);
            if (prFeature !== null) {
                prVectorSource.removeFeature(prFeature);
            }
        }
    });
    connection.on('DeleteSR', function (srNumber) {
        if (srNumber !== null) {
            var srFeature = srVectorSource.getFeatureById(srNumber);
            if (srFeature !== null) {
                srVectorSource.removeFeature(srFeature);
            }
        }
    });
    connection.on('DeleteCOI', function (coiNumber) {
        if (coiNumber !== null) {
            var coiFeature = coiVectorSource.getFeatureById(coiNumber);
            if (coiFeature !== null) {
                coiVectorSource.removeFeature(coiFeature);
            }
        }
    });
    connection.on('DeleteAR', function (arNumber) {
        if (arNumber !== null) {
            var arFeature = ARVectorSource.getFeatureById(arNumber);
            if (arFeature !== null) {
                ARVectorSource.removeFeature(arFeature);
            }
        }
    });
    connection.on('PushNotification', function (notifications) {
        count++;
        $('#noti_badge').css({ "display": "inherit" });
        $("#noti_badge").html(count);
        $("#noti").prepend(
            '<div id="notiItem-' + notifications.id + '" class="kt-notification__item" style="padding-bottom:0px;">' +
            '<div class="kt-notification__item-icon">' +
            '<i class="flaticon-feed kt-font-success"></i>' +
            '</div>' +
            '<div class="kt-notification__item-details">' +
            '<div class="kt-notification__item-title">' + notifications.notificationContent + '</div>' +
            '</div>' +
            '</div>' +
            '<div style="text-align:right;">' +
            "<button type='button' class='btn btn-primary btn-sm ml-3' onclick=\"viewReport(" + notifications.reportId + ",'" + notifications.notificationType + "')\">View</button>" +
            '<button type="button" style="margin-right: 1.5rem;" class="btn btn-primary btn-sm ml-3" onclick="markRead(' + notifications.id + ')">Hide</button>' +
            '</div>');
    });
    connection.on('PushNewsFeed', function (newsFeed) {
        console.log(newsFeed);
    });
    connection.on('PushDelete', function (id) {
        console.log(id);
    });
    connection.on('PushDrawing', function (dCoord) {
        console.log(dCoord);
    });
    connection.on('PushNatureOfThreat', function (threatLevel) {
        $.ajax({
            url: 'Canvas?handler=AllNatureOfThreats',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                thrLevelDatatable.destroy();
                threatLevelsDatatable(response);

                var modalContent = $('#threatLevelsModal').find('.modal-content');
                thrLevelDatatable.spinnerCallback(true, modalContent);
                thrLevelDatatable.reload();
                thrLevelDatatable.on('kt-datatable--on-layout-updated', function () {
                    thrLevelDatatable.show();
                    thrLevelDatatable.spinnerCallback(false, modalContent);
                    thrLevelDatatable.redraw();
                });
            }
        });
    });
    connection.on('PushCOIType', function (COIType) {
        $.ajax({
            url: 'Canvas?handler=COITypes',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                coiTypeDatatable.destroy();
                coiTypesDatatable(response);

                var modalContent = $('#COITypesModal').find('.modal-content');
                coiTypeDatatable.spinnerCallback(true, modalContent);
                coiTypeDatatable.reload();
                coiTypeDatatable.on('kt-datatable--on-layout-updated', function () {
                    coiTypeDatatable.show();
                    coiTypeDatatable.spinnerCallback(false, modalContent);
                    coiTypeDatatable.redraw();
                });
            }
        });
    });
    connection.on('PushTemplateType', function (TemplateType) {
        $.ajax({
            url: 'Canvas?handler=TemplateTypes',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                templateTypeDatatable.destroy();
                templateTypesDatatable(response);

                var modalContent = $('#TemplateTypesModal').find('.modal-content');
                templateTypeDatatable.spinnerCallback(true, modalContent);
                templateTypeDatatable.reload();
                templateTypeDatatable.on('kt-datatable--on-layout-updated', function () {
                    templateTypeDatatable.show();
                    templateTypeDatatable.spinnerCallback(false, modalContent);
                    templateTypeDatatable.redraw();
                });
            }
        });
    });
    connection.on('PushStakeholder', function (subscriber) {
        $.ajax({
            url: 'Canvas?handler=AllStakeholders',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                subsDatatable.destroy();
                stakeholdersDatatable(response);

                var modalContent = $('#StakeholdersModal').find('.modal-content');
                subsDatatable.spinnerCallback(true, modalContent);
                subsDatatable.reload();
                subsDatatable.on('kt-datatable--on-layout-updated', function () {
                    subsDatatable.show();
                    subsDatatable.spinnerCallback(false, modalContent);
                    subsDatatable.redraw();
                });
            }
        });
    });
    connection.on('PushUser', function (appUser) {
        $.ajax({
            url: 'Canvas?handler=AllUsers',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                usrsDatatable.destroy();
                usersDatatable(response);

                var modalContent = $('#UsersModal').find('.modal-content');
                usrsDatatable.spinnerCallback(true, modalContent);
                usrsDatatable.reload();
                usrsDatatable.on('kt-datatable--on-layout-updated', function () {
                    usrsDatatable.show();
                    usrsDatatable.spinnerCallback(false, modalContent);
                    usrsDatatable.redraw();
                });
            }
        });
    });
    connection.on('PushUserType', function (userType) {
        $.ajax({
            url: 'Canvas?handler=AllUserTypes',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                usrTypeDatatable.destroy();
                userTypesDatatable(response);

                var modalContent = $('#UserTypesModal').find('.modal-content');
                usrTypeDatatable.spinnerCallback(true, modalContent);
                usrTypeDatatable.reload();
                usrTypeDatatable.on('kt-datatable--on-layout-updated', function () {
                    usrTypeDatatable.show();
                    usrTypeDatatable.spinnerCallback(false, modalContent);
                    usrTypeDatatable.redraw();
                });
            }
        });
    });
    connection.on('PushNewsFeedType', function (newsFeedType) {
        $.ajax({
            url: 'Canvas?handler=AllNewsFeedTypes',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                nwsFeedTypeDatatable.destroy();
                newsFeedTypesDatatable(response);

                var modalContent = $('#NewsFeedTypesModal').find('.modal-content');
                nwsFeedTypeDatatable.spinnerCallback(true, modalContent);
                nwsFeedTypeDatatable.reload();
                nwsFeedTypeDatatable.on('kt-datatable--on-layout-updated', function () {
                    nwsFeedTypeDatatable.show();
                    nwsFeedTypeDatatable.spinnerCallback(false, modalContent);
                    nwsFeedTypeDatatable.redraw();
                });
            }
        });
    });
    connection.on('PushCOIStatus', function (COIStatus) {
        $.ajax({
            url: 'Canvas?handler=AllCOIStatuses',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                coiStDatatable.destroy();
                coiStatusesDatatable(response);

                var modalContent = $('#COIStatusesModal').find('.modal-content');
                coiStDatatable.spinnerCallback(true, modalContent);
                coiStDatatable.reload();
                coiStDatatable.on('kt-datatable--on-layout-updated', function () {
                    coiStDatatable.show();
                    coiStDatatable.spinnerCallback(false, modalContent);
                    coiStDatatable.redraw();
                });
            }
        });
    });
    connection.on('PushInfoConLevel', function (infoConLevel) {
        $.ajax({
            url: 'Canvas?handler=AllInfoConLevels',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                infoConDatatable.destroy();
                infoConLevelsDatatable(response);

                var modalContent = $('#InfoConLevelsModal').find('.modal-content');
                infoConDatatable.spinnerCallback(true, modalContent);
                infoConDatatable.reload();
                infoConDatatable.on('kt-datatable--on-layout-updated', function () {
                    infoConDatatable.show();
                    infoConDatatable.spinnerCallback(false, modalContent);
                    infoConDatatable.redraw();
                });
            }
        });
    });
    connection.on('PushAAASIncident', function (incident) {
        console.log(incident);
    });
    connection.on('PushAAASSOS', function (AAASOS) {
        console.log(AAASOS);
    });
    connection.on('PushNotes', function (Notes) {
        $.ajax({
            url: 'Canvas?handler=AllNotes',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                noteDatatable.destroy();
                NotesDatatable(response);

                var modalContent = $('#NotesModal').find('.modal-content');
                noteDatatable.spinnerCallback(true, modalContent);
                noteDatatable.reload();
                noteDatatable.on('kt-datatable--on-layout-updated', function () {
                    noteDatatable.show();
                    noteDatatable.spinnerCallback(false, modalContent);
                    noteDatatable.redraw();
                });
            }
        });
    });

});

$(document).ready(function () {
    initializeMap();
    loadLayerSwitcher();
    loadSearchFeature();
    createContextMenu();
    showPopupOnHover();
    getAllDrawings();
    getAllPRs();
    getAllSRs();
    getAllCOIs();
    getAllARs();
    getAllLRs();
    getAllDRs();
    getAllNotifications();
    getAllNewsFeed();
});

function initializeMap() {
    s57Maps = new ol.layer.Group({
        title: 'S57 Maps',
        type: 'base',
        combine: true,
        visible: true,
        layers: [
            new ol.layer.Vector({
                title: 'Gwadar Detail',
                visible: false,
                style: function (feature) {
                    return new ol.style.Style({
                        fill: new ol.style.Fill({
                            color: feature.get("CUSTOM_FILL")
                        }),
                        stroke: new ol.style.Stroke({
                            color: "#666",
                            width: 1.5
                        }),
                        text: new ol.style.Text({
                            text: feature.get("OBJNAM"),
                            font: "12px Calibri,sans-serif",
                            fill: new ol.style.Fill({
                                color: "#333"
                            })
                        })
                    });
                },
                source: new ol.source.Vector({
                    url: "assets/vectormaps/GwadarDetail.json",
                    format: new ol.format.GeoJSON()
                })
            }),
            new ol.layer.Vector({
                title: 'Gwadar Medium',
                visible: false,
                style: function (feature) {
                    return new ol.style.Style({
                        fill: new ol.style.Fill({
                            color: feature.get("CUSTOM_FILL")
                        }),
                        stroke: new ol.style.Stroke({
                            color: "#666",
                            width: 1.5
                        }),
                        //text: new ol.style.Text({
                        //    text: feature.get("Name"),
                        //    font: "12px Calibri,sans-serif",
                        //    fill: new ol.style.Fill({
                        //        color: "#333"
                        //    })
                        //})
                    });
                },
                source: new ol.source.Vector({
                    url: "assets/vectormaps/GwadarMedium.json",
                    format: new ol.format.GeoJSON()
                })
            }),

            new ol.layer.Vector({
                title: 'Karachi Harbour',
                visible: false,
                style: function (feature) {
                    return new ol.style.Style({
                        fill: new ol.style.Fill({
                            color: feature.get("CUSTOM_FILL")
                        }),
                        stroke: new ol.style.Stroke({
                            color: "#666",
                            width: 1.5
                        }),
                        text: new ol.style.Text({
                            text: feature.get("OBJNAM"),
                            font: "12px Calibri,sans-serif",
                            fill: new ol.style.Fill({
                                color: "#333"
                            })
                        })
                    });
                },
                source: new ol.source.Vector({
                    url: "assets/vectormaps/KarachiHarbour.json",
                    format: new ol.format.GeoJSON()
                })
            }),

            new ol.layer.Vector({
                title: 'Karachi Medium',
                visible: false,
                style: function (feature) {
                    return new ol.style.Style({
                        fill: new ol.style.Fill({
                            color: feature.get("CUSTOM_FILL")
                        }),
                        stroke: new ol.style.Stroke({
                            color: "#666",
                            width: 1.5
                        }),
                        text: new ol.style.Text({
                            text: feature.get("OBJNAM"),
                            font: "12px Calibri,sans-serif",
                            fill: new ol.style.Fill({
                                color: "#333"
                            })
                        })
                    });
                },
                source: new ol.source.Vector({
                    url: "assets/vectormaps/KarachiMedium.json",
                    format: new ol.format.GeoJSON()
                })
            }),

            new ol.layer.Vector({
                title: 'Korangi Detail',
                visible: true,
                style: function (feature) {
                    return new ol.style.Style({
                        fill: new ol.style.Fill({
                            color: feature.get("CUSTOM_FILL")
                        }),
                        stroke: new ol.style.Stroke({
                            color: "#666",
                            width: 1.5
                        }),
                        text: new ol.style.Text({
                            text: feature.get("OBJNAM"),
                            font: "12px Calibri,sans-serif",
                            fill: new ol.style.Fill({
                                color: "#333"
                            })
                        })
                    });
                },
                source: new ol.source.Vector({
                    url: "assets/vectormaps/KorangiDetail.json",
                    format: new ol.format.GeoJSON()
                })
            }),

            new ol.layer.Vector({
                title: 'Pasni',
                visible: false,
                style: function (feature) {
                    return new ol.style.Style({
                        //fill: new ol.style.Fill({
                        //    color: feature.get("CUSTOM_FILL")
                        //}),
                        stroke: new ol.style.Stroke({
                            color: "#F6AB94",
                            width: 1.5
                        }),
                        //text: new ol.style.Text({
                        //    text: feature.get("Name"),
                        //    font: "12px Calibri,sans-serif",
                        //    fill: new ol.style.Fill({
                        //        color: "#333"
                        //    })
                        //})
                    });
                },
                source: new ol.source.Vector({
                    url: "assets/vectormaps/Pasni.json",
                    format: new ol.format.GeoJSON()
                })
            })

        ]
    });

    waterColorMap = new ol.layer.Group({
        title: 'Water color with labels',
        type: 'base',
        combine: true,
        visible: false,
        layers: [
            new ol.layer.Tile({
                title: 'Watercolor Map',
                visible: true,
                source: new ol.source.Stamen({
                    layer: 'watercolor'
                })
            }),
            new ol.layer.Tile({
                title: 'Terrain-labels',
                visible: true,
                source: new ol.source.Stamen({
                    layer: 'terrain-labels'
                })
            })
        ]
    });

    vectorMap = new ol.layer.Vector({
        title: 'Vector Map',
        type: 'base',
        visible: true,
        source: new ol.source.Vector({
            url: "assets/vectormaps/countries.json",
            format: new ol.format.GeoJSON()
        }),
        style: function (feature) {
            return new ol.style.Style({
                fill: new ol.style.Fill({
                    color: "#F1EFE9"
                }),
                stroke: new ol.style.Stroke({
                    color: "#F6AB94",
                    width: 1.5
                }),
                text: new ol.style.Text({
                    text: feature.get("name"),
                    font: "12px Calibri,sans-serif",
                    fill: new ol.style.Fill({
                        color: "#333"
                    })
                })
            });
        }
    });

    openStreetMap = new ol.layer.Tile({
        title: 'Open Street Map',
        type: 'base',
        visible: false,
        source: new ol.source.OSM()
    });

    googleMap = new ol.layer.Tile({
        title: 'Google Map',
        type: 'base',
        visible: false,
        source: new ol.source.XYZ({
            url: "https://mt1.google.com/vt/lyrs=s&x={x}&y={y}&z={z}"
        })
    });

    drawingVectorSource = new ol.source.Vector();
    drawingVectorLayer = new ol.layer.Vector({
        title: 'Drawings',
        source: drawingVectorSource,
        style: new ol.style.Style({
            fill: new ol.style.Fill({
                color: "rgba(255, 255, 255, 0.2)"
            }),
            stroke: new ol.style.Stroke({
                color: '#F6AB94',
                width: 2
            })
        })
    });

    localAISVectorSource = new ol.source.Vector();

    csnAISVectorSource = new ol.source.Vector();

    furunoVectorSource = new ol.source.Vector();

    Tracks = new ol.layer.Group({
        title: 'Tracks',
        combine: true,
        visible: true,
        layers: [
            new ol.layer.Vector({
                title: 'FURUNO',
                source: furunoVectorSource
            }),
            new ol.layer.Vector({
                title: 'CSN AIS',
                source: csnAISVectorSource
            }),
            new ol.layer.Vector({
                title: 'Local AIS',
                source: localAISVectorSource
            })
        ]
    });

    prVectorSource = new ol.source.Vector();
    prVectorLayer = new ol.layer.Vector({
        title: 'Preliminary Reports',
        source: prVectorSource,
        style: new ol.style.Style({
            image: new ol.style.Icon({
                src: 'assets/images/marker.webp',
                offsetOrigin: 'top-right',
                offset: [145, 150],
                size: [30, 30]
            })
        })
    });

    LRVectorSource = new ol.source.Vector();
    LRVectorLayer = new ol.layer.Vector({
        title: 'Lost Reports',
        source: LRVectorSource,
        style: new ol.style.Style({
            image: new ol.style.Icon({
                src: 'assets/images/marker.webp',
                offsetOrigin: 'top-right',
                offset: [85, 150],
                size: [30, 30]
            })
        })
    });

    ARVectorSource = new ol.source.Vector();
    ARVectorLayer = new ol.layer.Vector({
        title: 'Amplification Reports',
        source: ARVectorSource,
        style: new ol.style.Style({
            image: new ol.style.Icon({
                src: 'assets/images/marker.webp',
                offsetOrigin: 'top-right',
                offset: [25, 150],
                size: [30, 30]
            })
        })
    });

    coiVectorSource = new ol.source.Vector();
    coiVectorLayer = new ol.layer.Vector({
        title: 'COIs',
        source: coiVectorSource,
        style: new ol.style.Style({
            image: new ol.style.Icon({
                src: 'assets/images/marker.webp',
                offsetOrigin: 'top-right',
                offset: [115, 150],
                size: [30, 30]
            })
        })
    });

    srVectorSource = new ol.source.Vector();
    srVectorLayer = new ol.layer.Vector({
        title: 'Subsequent Reports',
        source: srVectorSource,
        style: new ol.style.Style({
            image: new ol.style.Icon({
                src: 'assets/images/marker.webp',
                offsetOrigin: 'top-right',
                offset: [55, 150],
                size: [30, 30]
            })
        })
    });

    drVectorSource = new ol.source.Vector();
    drVectorLayer = new ol.layer.Vector({
        title: 'Drop Reports',
        source: drVectorSource,
        style: new ol.style.Style({
            image: new ol.style.Icon({
                src: 'assets/images/marker.webp',
                offsetOrigin: 'top-left',
                offset: [25, 150],
                size: [30, 30]
            })
        })
    });

    hoverPopupOverlay = new ol.Overlay({
        element: document.getElementById('hoverpopup'),
        autoPan: true
    });

    clickPopupOverlay = new ol.Overlay({
        element: document.getElementById('clickpopup'),
        autoPan: true
    });

    searchPopupOverlay = new ol.Overlay({
        element: document.getElementById('searchpopup'),
        autoPan: true
    });

    view = new ol.View({
        center: ol.proj.fromLonLat([66.9742, 24.8400]),
        zoom: 10
    });

    map = new ol.Map({
        controls: ol.control.defaults({ zoom: false, rotate: false, attribution: false }),
        layers: [
            new ol.layer.Group({
                title: 'Base maps',
                layers: [googleMap, vectorMap, waterColorMap, openStreetMap, s57Maps]
            }),
            new ol.layer.Group({
                title: 'Overlays',
                fold: 'open',
                layers: [drawingVectorLayer, drVectorLayer, LRVectorLayer, ARVectorLayer, coiVectorLayer, srVectorLayer, prVectorLayer, Tracks]
            })
        ],
        overlays: [clickPopupOverlay, searchPopupOverlay, hoverPopupOverlay],
        view: view,
        target: 'map',
        interactions: ol.interaction.defaults({ doubleClickZoom: false })
    });

    map.on("click", function (e) {
        if (typeof zoomType !== 'undefined') {
            if (zoomType === "zoom-in") {
                view = map.getView();
                zoom = view.getZoom();
                view.animate({
                    zoom: zoom + 1,
                    duration: 100
                });
            }
            else if (zoomType === "zoom-out") {
                view = map.getView();
                zoom = view.getZoom();
                view.animate({
                    zoom: zoom - 1,
                    duration: 100
                });
            }
        }
        else {
            map.forEachFeatureAtPixel(e.pixel, function (feature, layer) {
                if (feature && feature.get('trackType') === 'Local AIS' || feature && feature.get('trackType') === 'CSN AIS') {

                    if (feature.getProperties().IsLloydInfoPresent) {

                        var clickCloser = document.getElementById('clickpopup-closer');
                        var clickContent = document.getElementById('clickpopup-content');
                        clickCloser.onclick = function () {
                            clickPopupOverlay.setPosition(undefined);
                            clickCloser.blur();
                            return false;
                        };

                        var data = feature.getProperties().LlydDetail;
                        if (data !== null) {
                            var flagCode = data.flagCode;
                            if (data.flagCode === 'CON') {
                                flagCode = 'CON1';
                            }
                            var coordinate = e.coordinate;
                            clickContent.innerHTML = '<div class="kt-widget kt-widget--user-profile-2">' +
                                '<div class="kt-widget__head m-0">' +
                                '<img style="height:30px;width:35px;margin-right:5px;" src="assets/images/CountryFlags/' + flagCode + '.jpg" />' +
                                '<div class="kt-widget__info p-0">' +
                                '<span class="kt-widget__username">' +
                                '' + data.shipName + '' +
                                '</span>' +
                                '<span class="kt-widget__desc p-0">' +
                                '' + data.shipTypeLevel2 + '' +
                                '</span>' +
                                '</div>' +
                                '</div>';
                            if (data.photoPresent === false) {
                                clickContent.innerHTML += '<div id="defaultShipImg" class="shipImg">' +
                                    '<img src="assets/images/demoShip.png" />' +
                                    '</div>';
                            }
                            else {
                                clickContent.innerHTML += '<div id="shipImg" class="shipImg">' +
                                    '<img src="data:image/jpg;base64, ' + data.photoContent + '" />' +
                                    '</div>';
                            }

                            clickContent.innerHTML += '<div class="kt-widget__body">' +
                                '<div class="kt-widget__item">' +
                                '<div class="kt-widget__contact p-0">' +
                                '<span class="kt-widget__label m-0">IMO:</span>' +
                                '<span class="kt-widget__data m-0">' + data.imo + '</span>' +
                                '</div>' +
                                '<div class="kt-widget__contact p-0">' +
                                '<span class="kt-widget__label m-0">MMSI:</span>' +
                                '<span class="kt-widget__data m-0">' + data.mmsi + '</span>' +
                                '</div>' +
                                '<div class="kt-widget__contact p-0">' +
                                '<span class="kt-widget__label m-0">Ex-Name:</span>' +
                                '<span class="kt-widget__data m-0">' + data.exName + '</span>' +
                                '</div>' +
                                '<div class="kt-widget__contact p-0">' +
                                '<span class="kt-widget__label m-0">Status:</span>' +
                                '<span class="kt-widget__data m-0">' + data.shipStatus + '</span>' +
                                '</div>' +
                                '<div class="kt-widget__contact p-0">' +
                                '<span class="kt-widget__label m-0">Country:</span>' +
                                '<span class="kt-widget__data m-0">' + data.flagName + '</span>' +
                                '</div>' +
                                '<div class="kt-widget__contact p-0">' +
                                '<span class="kt-widget__label m-0">Call Sign:</span>' +
                                '<span class="kt-widget__data m-0">' + data.callSign + '</span>' +
                                '</div>' +
                                '<div class="kt-widget__contact p-0">' +
                                '<span class="kt-widget__label m-0">Registry Port:</span>' +
                                '<span class="kt-widget__data m-0">' + data.portOfRegistry + '</span>' +
                                '</div>' +
                                '<div class="kt-widget__contact p-0">' +
                                '<span class="kt-widget__label m-0">Port Code:</span>' +
                                '<span class="kt-widget__data m-0">' + data.portOfRegistryCode + '</span>' +
                                '</div>' +
                                '<div class="kt-widget__contact p-0">' +
                                '<span class="kt-widget__label m-0">Gross Tonnage:</span>' +
                                '<span class="kt-widget__data m-0">' + data.grossTonnage + '</span>' +
                                '</div>' +
                                '<div class="kt-widget__contact p-0">' +
                                '<span class="kt-widget__label m-0">Dead Weight:</span>' +
                                '<span class="kt-widget__data m-0">' + data.deadWeight + '</span>' +
                                '</div>' +
                                '<div class="kt-widget__contact p-0">' +
                                '<span class="kt-widget__label m-0">Sub Group:</span>' +
                                '<span class="kt-widget__data m-0">' + data.shipTypeLevel5SubGroup + '</span>' +
                                '</div>' +
                                '<div class="kt-widget__contact p-0">' +
                                '<span class="kt-widget__label m-0">Sub Type:</span>' +
                                '<span class="kt-widget__data m-0">' + data.shipTypeLevel5SubType + '</span>' +
                                '</div>' +
                                '<div class="kt-widget__contact p-0">' +
                                '<span class="kt-widget__label m-0">Type Group:</span>' +
                                '<span class="kt-widget__data m-0">' + data.shipTypeGroup + '</span>' +
                                '</div>' +
                                '<div class="kt-widget__contact p-0">' +
                                '<span class="kt-widget__label m-0">Manager:</span>' +
                                '<span class="kt-widget__data m-0">' + data.shipManager + '</span>' +
                                '</div>' +
                                '<div class="kt-widget__contact p-0">' +
                                '<span class="kt-widget__label m-0">Owner:</span>' +
                                '<span class="kt-widget__data m-0">' + data.registeredOwner + '</span>' +
                                '</div>' +
                                '<div class="kt-widget__contact p-0">' +
                                '<span class="kt-widget__label m-0">Beneficial Owner:</span>' +
                                '<span class="kt-widget__data m-0">' + data.groupBeneficialOwner + '</span>' +
                                '</div>' +
                                '<div class="kt-widget__contact p-0">' +
                                '<span class="kt-widget__label m-0">Operator:</span>' +
                                '<span class="kt-widget__data m-0">' + data.operator + '</span>' +
                                '</div>' +
                                '</div>' +
                                '</div>' +
                                '</div>';
                            clickPopupOverlay.setPosition(coordinate);
                        }

                        else {
                            clickPopupOverlay.setPosition(undefined);
                        }
                    }


                    //var mmsi = feature.getProperties().trackNumber;
                    //var imo = feature.getProperties().imo;
                    //fetch('Canvas?handler=Ship&mmsi=' + mmsi + '&imo=' + imo, {
                    //    method: 'get'
                    //}).then(function (response) {
                    //    if (response.status === 200) {
                    //        response.json().then(function (data) {
                    //            var clickCloser = document.getElementById('clickpopup-closer');
                    //            var clickContent = document.getElementById('clickpopup-content');
                    //            clickCloser.onclick = function () {
                    //                clickPopupOverlay.setPosition(undefined);
                    //                clickCloser.blur();
                    //                return false;
                    //            };
                    //            if (data !== null) {
                    //                var flagCode = data.flagCode;
                    //                if (data.flagCode === 'CON') {
                    //                    flagCode = 'CON1';
                    //                }
                    //                var coordinate = e.coordinate;
                    //                clickContent.innerHTML = '<div class="kt-widget kt-widget--user-profile-2">' +
                    //                    '<div class="kt-widget__head m-0">' +
                    //                    '<img style="height:30px;width:35px;margin-right:5px;" src="assets/images/CountryFlags/' + flagCode + '.jpg" />' +
                    //                    '<div class="kt-widget__info p-0">' +
                    //                    '<span class="kt-widget__username">' +
                    //                    '' + data.shipName + '' +
                    //                    '</span>' +
                    //                    '<span class="kt-widget__desc p-0">' +
                    //                    '' + data.shipTypeLevel2 + '' +
                    //                    '</span>' +
                    //                    '</div>' +
                    //                    '</div>';
                    //                if (data.photoPresent === false) {
                    //                    clickContent.innerHTML += '<div id="defaultShipImg" class="shipImg">' +
                    //                        '<img src="assets/images/demoShip.png" />' +
                    //                        '</div>';
                    //                }
                    //                else {
                    //                    clickContent.innerHTML += '<div id="shipImg" class="shipImg">' +
                    //                        '<img src="data:image/jpg;base64, ' + data.photoContent + '" />' +
                    //                        '</div>';
                    //                }

                    //                clickContent.innerHTML += '<div class="kt-widget__body">' +
                    //                    '<div class="kt-widget__item">' +
                    //                    '<div class="kt-widget__contact p-0">' +
                    //                    '<span class="kt-widget__label m-0">IMO:</span>' +
                    //                    '<span class="kt-widget__data m-0">' + data.imo + '</span>' +
                    //                    '</div>' +
                    //                    '<div class="kt-widget__contact p-0">' +
                    //                    '<span class="kt-widget__label m-0">MMSI:</span>' +
                    //                    '<span class="kt-widget__data m-0">' + data.mmsi + '</span>' +
                    //                    '</div>' +
                    //                    '<div class="kt-widget__contact p-0">' +
                    //                    '<span class="kt-widget__label m-0">Ex-Name:</span>' +
                    //                    '<span class="kt-widget__data m-0">' + data.exName + '</span>' +
                    //                    '</div>' +
                    //                    '<div class="kt-widget__contact p-0">' +
                    //                    '<span class="kt-widget__label m-0">Status:</span>' +
                    //                    '<span class="kt-widget__data m-0">' + data.shipStatus + '</span>' +
                    //                    '</div>' +
                    //                    '<div class="kt-widget__contact p-0">' +
                    //                    '<span class="kt-widget__label m-0">Country:</span>' +
                    //                    '<span class="kt-widget__data m-0">' + data.flagName + '</span>' +
                    //                    '</div>' +
                    //                    '<div class="kt-widget__contact p-0">' +
                    //                    '<span class="kt-widget__label m-0">Call Sign:</span>' +
                    //                    '<span class="kt-widget__data m-0">' + data.callSign + '</span>' +
                    //                    '</div>' +
                    //                    '<div class="kt-widget__contact p-0">' +
                    //                    '<span class="kt-widget__label m-0">Registry Port:</span>' +
                    //                    '<span class="kt-widget__data m-0">' + data.portOfRegistry + '</span>' +
                    //                    '</div>' +
                    //                    '<div class="kt-widget__contact p-0">' +
                    //                    '<span class="kt-widget__label m-0">Port Code:</span>' +
                    //                    '<span class="kt-widget__data m-0">' + data.portOfRegistryCode + '</span>' +
                    //                    '</div>' +
                    //                    '<div class="kt-widget__contact p-0">' +
                    //                    '<span class="kt-widget__label m-0">Gross Tonnage:</span>' +
                    //                    '<span class="kt-widget__data m-0">' + data.grossTonnage + '</span>' +
                    //                    '</div>' +
                    //                    '<div class="kt-widget__contact p-0">' +
                    //                    '<span class="kt-widget__label m-0">Dead Weight:</span>' +
                    //                    '<span class="kt-widget__data m-0">' + data.deadWeight + '</span>' +
                    //                    '</div>' +
                    //                    '<div class="kt-widget__contact p-0">' +
                    //                    '<span class="kt-widget__label m-0">Sub Group:</span>' +
                    //                    '<span class="kt-widget__data m-0">' + data.shipTypeLevel5SubGroup + '</span>' +
                    //                    '</div>' +
                    //                    '<div class="kt-widget__contact p-0">' +
                    //                    '<span class="kt-widget__label m-0">Sub Type:</span>' +
                    //                    '<span class="kt-widget__data m-0">' + data.shipTypeLevel5SubType + '</span>' +
                    //                    '</div>' +
                    //                    '<div class="kt-widget__contact p-0">' +
                    //                    '<span class="kt-widget__label m-0">Type Group:</span>' +
                    //                    '<span class="kt-widget__data m-0">' + data.shipTypeGroup + '</span>' +
                    //                    '</div>' +
                    //                    '<div class="kt-widget__contact p-0">' +
                    //                    '<span class="kt-widget__label m-0">Manager:</span>' +
                    //                    '<span class="kt-widget__data m-0">' + data.shipManager + '</span>' +
                    //                    '</div>' +
                    //                    '<div class="kt-widget__contact p-0">' +
                    //                    '<span class="kt-widget__label m-0">Owner:</span>' +
                    //                    '<span class="kt-widget__data m-0">' + data.registeredOwner + '</span>' +
                    //                    '</div>' +
                    //                    '<div class="kt-widget__contact p-0">' +
                    //                    '<span class="kt-widget__label m-0">Beneficial Owner:</span>' +
                    //                    '<span class="kt-widget__data m-0">' + data.groupBeneficialOwner + '</span>' +
                    //                    '</div>' +
                    //                    '<div class="kt-widget__contact p-0">' +
                    //                    '<span class="kt-widget__label m-0">Operator:</span>' +
                    //                    '<span class="kt-widget__data m-0">' + data.operator + '</span>' +
                    //                    '</div>' +
                    //                    '</div>' +
                    //                    '</div>' +
                    //                    '</div>';
                    //                clickPopupOverlay.setPosition(coordinate);
                    //            }

                    //            else {
                    //                clickPopupOverlay.setPosition(undefined);
                    //            }
                    //        });
                    //    }
                    //    else if (response.status === 400) {
                    //        console.log('Not Found');
                    //        return;
                    //    }                       
                    //}).catch((err) => console.log(err));
                    return false;
                }

            });
        }
        e.preventDefault();
    });
}

function loadLayerSwitcher() {
    layerSwitcher = new ol.control.LayerSwitcher();
    map.addControl(layerSwitcher);
}

function loadSearchFeature() {
    search = new ol.control.SearchFeature({
        source: localAISVectorSource,
        property: 'trackNumber'
    });
    map.addControl(search);

    search.on('select', function (e) {
        var f = e.search;
        var p = f.getGeometry().getFirstCoordinate();
        view.animate({ center: p });
        showPopupOnSearch(f, p);
    });
}

function createContextMenu() {
    var contextmenu_items = [
        {
            text: 'Center map here',
            classname: 'bold',
            icon: 'assets/images/center.png',
            callback: center
        },
        {
            text: 'Create Preliminary Report',
            icon: 'assets/images/view_list.png',
            callback: createPR
        },
        {
            text: 'Add News Feed',
            icon: 'assets/images/view_list.png',
            callback: insertNews
        },
        //{
        //    text: 'Some Actions',
        //    icon: 'assets/images/view_list.png',
        //    items: [
        //        {
        //            text: 'Center map here',
        //            icon: 'assets/images/center.png',
        //            callback: center
        //        },
        //        {
        //            text: 'Add a Marker',
        //            icon: 'assets/images/pin_drop.png',
        //            callback: marker
        //        }
        //    ]
        //},
        //{
        //    text: 'Add a Marker',
        //    icon: 'assets/images/pin_drop.png',
        //    callback: marker
        //},
        //'-' // this is a separator
    ];

    var removeMarkerItem = {
        text: 'Remove this Feature',
        classname: 'marker',
        icon: 'assets/images/remove.png',
        callback: removeMarker
    };

    var trackItem = {
        text: 'Create Preliminary Report',
        classname: 'marker',
        icon: 'assets/images/view_list.png',
        callback: trackPR
    };

    var contextmenu = new ContextMenu({
        width: 200,
        items: contextmenu_items
    });
    map.addControl(contextmenu);


    contextmenu.on('open', function (evt) {
        var feature = map.forEachFeatureAtPixel(evt.pixel, function (ft, l) {
            return ft;
        });
        if (feature && feature.get('type') === 'removable' || feature && feature.get('type') === 'drawingFeature') {
            contextmenu.clear();
            removeMarkerItem.data = {
                marker: feature
            };
            contextmenu.push(removeMarkerItem);
        }

        else if (feature && feature.get('type') === 'Track') {
            contextmenu.clear();
            trackItem.data = {
                marker: feature
            };
            contextmenu.push(trackItem);
        }

        else {
            contextmenu.clear();
            contextmenu.extend(contextmenu_items);
            contextmenu.extend(contextmenu.getDefaultItems());
        }
    });

    //map.on('pointermove', function (e) {

    //    var pixel = map.getEventPixel(e.originalEvent);
    //    var hit = map.hasFeatureAtPixel(pixel);

    //    if (e.dragging) return;

    //    map.getTargetElement().style.cursor = hit ? 'pointer' : '';
    //});

    function elastic(t) {
        return Math.pow(2, -10 * t) * Math.sin((t - 0.075) * (2 * Math.PI) / 0.3) + 1;
    }

    function center(obj) {
        view.animate({
            duration: 700,
            easing: elastic,
            center: obj.coordinate
        });
    }

    function removeMarker(obj) {
        $.ajax({
            url: 'Canvas?handler=DeleteDrawing',
            type: 'GET',
            data: {
                drID: obj.data.marker.getProperties().drId
            },
            success: function (result) {
            }
        });

        drawingVectorLayer.getSource().removeFeature(obj.data.marker);
    }

    function trackPR(obj) {
        var placeholderElement = $('#modal-placeholder');
        $.get('Canvas?handler=PR').done(function (data) {
            placeholderElement.html(data);
            modalDraggable();

            $('#lat').val(obj.data.marker.getProperties().lat);
            $('#lon').val(obj.data.marker.getProperties().lon);
            $('#mmsi').val(obj.data.marker.getProperties().trackNumber);
            $('#course').val(obj.data.marker.getProperties().course);
            $('#speed').val(obj.data.marker.getProperties().speed);
            // $('#heading').val(obj.data.marker.getProperties().heading);

            bindDropdowns();
            placeholderElement.find('#PreliminaryReportModal').modal('show');
        });
    }

    function marker(obj) {
        var coord4326 = ol.proj.transform(obj.coordinate, 'EPSG:3857', 'EPSG:4326'),
            template = 'Coordinate is ({x} | {y})',
            iconStyle = new ol.style.Style({
                image: new ol.style.Icon({ scale: .6, src: 'assets/images/pin_drop.png' }),
                text: new ol.style.Text({
                    offsetY: 25,
                    text: ol.coordinate.format(coord4326, template, 2),
                    font: '15px Open Sans,sans-serif',
                    fill: new ol.style.Fill({ color: '#111' }),
                    stroke: new ol.style.Stroke({ color: '#eee', width: 2 })
                })
            }),
            feature = new ol.Feature({
                type: 'removable',
                geometry: new ol.geom.Point(obj.coordinate)
            });

        feature.setStyle(iconStyle);
        drawingVectorLayer.getSource().addFeature(feature);
    }

    function insertNews() {
        var placeholderElement = $('#modal-placeholder');
        $.get('Canvas?handler=NewsFeed').done(function (data) {
            placeholderElement.html(data);
            modalDraggable();
            bindDropdowns();
            placeholderElement.find('#NewsFeedModal').modal('show');
        });
    }

    function createPR(obj) {
        var placeholderElement = $('#modal-placeholder');
        $.get('Canvas?handler=PR').done(function (data) {
            placeholderElement.html(data);

            var coord4326 = ol.proj.transform(obj.coordinate, 'EPSG:3857', 'EPSG:4326');
            var hdms = ol.coordinate.toStringHDMS(ol.proj.toLonLat(obj.coordinate));
            console.log(hdms);
            $('#lat').val(coord4326[1]);
            $('#lon').val(coord4326[0]);
            $('.trackFields').hide();

            bindDropdowns();
            modalDraggable();
            placeholderElement.find('#PreliminaryReportModal').modal('show');
        });
    }
}

function getAllDrawings() {
    $.ajax({
        url: 'Canvas?handler=AllSubsDrawings',
        type: 'GET',
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            $.each(response, function (i, item) {
                if (item.drawingSource !== null && item.shapeId !== 4) {
                    var drSource = item.drawingSource;
                    drSource = JSON.parse(drSource.replace(/&quot;/g, '"'));
                    getDrawings(drSource);
                    drawingFeature.setProperties({
                        drId: item.drawingId
                    });
                }
                else {
                    var radiusInMeters = convertRadiusToMeters(item.circleRadiusUnitId, item.circleRadius);
                    circleFeature = new ol.Feature({ geometry: new ol.geom.Circle(ol.proj.transform([item.circleLongitude, item.circleLatitude], 'EPSG:4326', 'EPSG:3857'), radiusInMeters) });

                    drawingFeatureStyle = new ol.style.Style({
                        fill: new ol.style.Fill({
                            color: item.drawingFillColor
                        }),
                        stroke: new ol.style.Stroke({
                            color: item.drawingOutlineColor,
                            width: 2
                        }),
                        text: new ol.style.Text({
                            text: item.drawingName
                        })
                    });
                    circleFeature.setProperties({
                        type: 'drawingFeature',
                        drId: item.drawingId
                    });
                    circleFeature.setStyle(drawingFeatureStyle);
                    drawingVectorSource.addFeature(circleFeature);
                }
            });
        }
    });
}

function convertRadiusToMeters(radiusUnitId, radius) {
    if (radiusUnitId === 5)       //km
        return radius * 1000;
    if (radiusUnitId === 6)       //Nm
        return radius * 1852;
    return radius;
}

function plotPRMarker(PR) {
    if (PR.latitude >= -90 && PR.latitude <= 90 && PR.longitude >= -180 && PR.longitude <= 180) {
        prMarker = new ol.Feature({
            geometry: new ol.geom.Point(ol.proj.fromLonLat([PR.longitude, PR.latitude])),
            type: 'PRMarker',
            reportingDatetime: PR.reportingDatetime,
            prNumber: PR.prNumber,
            latitude: PR.latitude,
            longitude: PR.longitude,
            coiTypeName: PR.coiTypeName
        });
        prMarker.setId(PR.prNumber);
        prVectorSource.addFeature(prMarker);
    }
}

function plotDRMarker(DR) {
    if (DR.latitude >= -90 && DR.latitude <= 90 && DR.longitude >= -180 && DR.longitude <= 180) {
        drMarker = new ol.Feature({
            geometry: new ol.geom.Point(ol.proj.fromLonLat([DR.longitude, DR.latitude])),
            type: 'DRMarker',
            reportingDatetime: DR.reportingDatetime,
            prNumber: DR.prNumber,
            coiNumber: DR.coiNumber,
            latitude: DR.latitude,
            longitude: DR.longitude
        });
        drMarker.setId(DR.drNumber);
        drVectorSource.addFeature(drMarker);
    }
}

function getAllPRs() {
    $.ajax({
        url: 'Canvas?handler=AllPRs',
        type: 'GET',
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            $.each(response, function (i, item) {
                plotPRMarker(item);
            });
        }
    });
}

function getAllSRs() {
    $.ajax({
        url: 'Canvas?handler=AllSRs',
        type: 'GET',
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            $.each(response, function (i, item) {
                if (item.latitude >= -90 && item.latitude <= 90 && item.longitude >= -180 && item.longitude <= 180) {
                    srMarker = new ol.Feature({
                        geometry: new ol.geom.Point(ol.proj.fromLonLat([item.longitude, item.latitude])),
                        type: 'SRMarker',
                        reportingDatetime: item.reportingDatetime,
                        srNumber: item.srNumber,
                        prNumber: item.prNumber,
                        latitude: item.latitude,
                        longitude: item.longitude,
                        coiTypeName: item.coiTypeName
                    });
                    srMarker.setId(item.srNumber);
                    srVectorSource.addFeature(srMarker);
                }
            });
        }
    });
}

function getAllCOIs() {
    $.ajax({
        url: 'Canvas?handler=AllCOIs',
        type: 'GET',
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            $.each(response, function (i, item) {
                if (item.latitude >= -90 && item.latitude <= 90 && item.longitude >= -180 && item.longitude <= 180) {
                    coiMarker = new ol.Feature({
                        geometry: new ol.geom.Point(ol.proj.fromLonLat([item.longitude, item.latitude])),
                        type: 'COIMarker',
                        reportingDatetime: item.reportingDatetime,
                        coiNumber: item.coiNumber,
                        latitude: item.latitude,
                        longitude: item.longitude,
                        coiTypeName: item.coiTypeName
                    });
                    coiMarker.setId(item.coiNumber);
                    coiVectorSource.addFeature(coiMarker);
                }
            });
        }
    });
}

function getAllARs() {
    $.ajax({
        url: 'Canvas?handler=AllARs',
        type: 'GET',
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            $.each(response, function (i, item) {
                if (item.latitude >= -90 && item.latitude <= 90 && item.longitude >= -180 && item.longitude <= 180) {
                    ARMarker = new ol.Feature({
                        geometry: new ol.geom.Point(ol.proj.fromLonLat([item.longitude, item.latitude])),
                        type: 'ARMarker',
                        reportingDatetime: item.reportingDatetime,
                        arNumber: item.arNumber,
                        latitude: item.latitude,
                        longitude: item.longitude,
                        coiClassificationName: item.coiClassificationName
                    });
                    ARMarker.setId(item.arNumber);
                    ARVectorSource.addFeature(ARMarker);
                }
            });
        }
    });
}

function getAllLRs() {
    $.ajax({
        url: 'Canvas?handler=AllLRs',
        type: 'GET',
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            $.each(response, function (i, item) {
                if (item.latitude >= -90 && item.latitude <= 90 && item.longitude >= -180 && item.longitude <= 180) {
                    LRMarker = new ol.Feature({
                        geometry: new ol.geom.Point(ol.proj.fromLonLat([item.longitude, item.latitude])),
                        type: 'LRMarker',
                        reportingDatetime: item.reportingDatetime,
                        latitude: item.latitude,
                        longitude: item.longitude
                    });
                    LRMarker.setId(item.id);
                    LRVectorSource.addFeature(LRMarker);
                }
            });
        }
    });
}

function getAllDRs() {
    $.ajax({
        url: 'Canvas?handler=AllDRs',
        type: 'GET',
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            $.each(response, function (i, item) {
                plotDRMarker(item);
            });
        }
    });
}

function getAllNotifications() {
    $.ajax({
        url: 'Canvas?handler=AllNotifications',
        type: 'GET',
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            count = Object.keys(response).length;
            if (count > 0) {
                $('#noti_badge').css({ "display": "inherit" });
                $("#noti_badge").html(count);
            }
            else {
                $('#noti_badge').hide();
            }
            $.each(response, function (i, item) {
                $('#noti').append(
                    '<div id="notiItem-' + item.id + '" class="kt-notification__item" style="padding-bottom:0px;">' +
                    '<div class="kt-notification__item-icon">' +
                    '<i class="flaticon-feed kt-font-success"></i>' +
                    '</div>' +
                    '<div class="kt-notification__item-details">' +
                    '<div class="kt-notification__item-title">' + item.notificationContent + '</div>' +
                    '</div>' +                   
                    '</div>' +
                    '<div style="text-align:right;">' +                    
                    "<button type='button' class='btn btn-primary btn-sm ml-3' onclick=\"viewReport(" + item.reportId + ",'" + item.notificationType + "')\">View</button>" +
                    '<button type="button" style="margin-right: 1.5rem;" class="btn btn-primary btn-sm ml-3" onclick="markRead(' + item.id + ')">Hide</button>' +
                    '</div>');
            });
        }
    });
}

function getWeather() {
    $.ajax({
        url: 'Canvas?handler=WeatherInfo',
        type: 'GET',
        contentType: "application/json",
        dataType: "json",
        success: function (item) {
            $('#weatherDropdown').empty();
            $('#weatherDropdown').append(
                '<div class="kt-mycart">' +
                '<div class="kt-mycart__head kt-head" style="background-image: url(Matronic/assets/media/misc/bg-1.jpg)">' +
                '<div class="kt-mycart__info">' +
                '<span class="kt-mycart__icon">' +
                '<i class="la la-sun-o kt-font-success"></i>' +
                '</span>' +

                '<h3 class="kt-mycart__title" style="width:100%; display:inline-block; text-align:center">' + item.name + ', ' + item.country + ' Weather</h3>' +
                '</div>' +
                '</div>' +
                '</div >' +
                '<div class="kt-quick-search kt-quick-search--dropdown kt-quick-search--result-compact">' +
                '<div class="input-group">' +
                '<div class="kt-notification__item" style="width:100%">' +
                '<div class="kt-notification__item-details">' +
                '<div class="kt-notification__item-title">' +
                '<h3 style="text-align: center;">' + item.main + ' (' + item.temp + '° <span> C</span>)</h3>' +
                '<h5 style="text-align: center;"><span>' + item.description + '</span></h5>' +
                '<div class="weather-subsection"><label>Min Temp: ' + item.tempMin + '°</label><span>Max Temp: ' + item.tempMax + '°</span></div>' +
                '<div class="weather-subsection"><label>Wind Speed: ' + item.speed + ' Km/h</label><span>Wind Degree: ' + item.deg + '°</span></div>' +
                '<div class="weather-subsection"><label>Pressure: ' + item.pressure + ' hPa</label><span>Humidity: ' + item.humidity + '%</span></div>' +
                '<div class="weather-subsection"><label>Sunrise: ' + item.sunrise + ' </label><span>Sunset: ' + item.sunset + '</span></div>' +
                '<div class="weather-subsection"><label>Pressure: ' + item.pressure + ' hPa</label><span>Humidity: ' + item.humidity + '%</span></div>' +

                '<div style="display:inline-block; text-align:center; width:100%">Updated as of ' + item.dt + '</div>' +
                '</div>' +
                '</div>' +

                '</div>' +
                '</div>' +
                '<div class="kt-quick-search__wrapper kt-scroll" data-scroll="true" data-height="325" data-mobile-height="200">' +
                '</div>' +
                '</div>');
        }
    });
}

function getAllNewsFeed() {
    $.ajax({
        url: 'Canvas?handler=AllNewsFeed',
        type: 'GET',
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            $.each(response, function (i, item) {
                $('#maritimeNews').append(
                    '<li style="display:inline;margin-left:10px;">[' + item.newsFeedTypeName + '] ' + item.newsFeedTitle + ' - <a href="javascript:;" onclick="openNewsWindow(\'' + item.newsFeedDescription + '\')" style="margin-right:10px;">Read more</a> | </li>');
            });
        }
    });
}

//has to declare global so that map can add and remove overlays
var helpTooltipElement;
var measureTooltipElement;
var helpTooltip;

function switchMarker(shipType, trackCourse, opacity) {
    var trackMarkerStyle;
    switch (shipType) {
        case "Cargo Vessels": {
            trackMarkerStyle = new ol.style.Style({
                image: new ol.style.Icon({
                    src: 'assets/images/marker.webp',
                    offsetOrigin: 'top-right',
                    offset: [125, 175],
                    size: [30, 30],
                    rotation: trackCourse,
                    opacity: opacity
                })
            });
            break;
        }
        case "Tankers": {
            trackMarkerStyle = new ol.style.Style({
                image: new ol.style.Icon({
                    src: 'assets/images/marker.webp',
                    offsetOrigin: 'top-right',
                    offset: [170, 175],
                    size: [30, 30],
                    rotation: trackCourse,
                    opacity: opacity
                })
            });
            break;
        }
        case "Passenger Vessels": {
            trackMarkerStyle = new ol.style.Style({
                image: new ol.style.Icon({
                    src: 'assets/images/marker.webp',
                    offsetOrigin: 'top-left',
                    offset: [0, 175],
                    size: [30, 30],
                    rotation: trackCourse,
                    opacity: opacity
                })
            });
            break;
        }
        case "High Speed Craft": {
            trackMarkerStyle = new ol.style.Style({
                image: new ol.style.Icon({
                    src: 'assets/images/marker.webp',
                    offsetOrigin: 'top-right',
                    offset: [145, 175],
                    size: [30, 30],
                    rotation: trackCourse,
                    opacity: opacity
                })
            });
            break;
        }
        case "Tugs and Special Craft": {
            trackMarkerStyle = new ol.style.Style({
                image: new ol.style.Icon({
                    src: 'assets/images/marker.webp',
                    offsetOrigin: 'top-right',
                    offset: [65, 175],
                    size: [30, 30],
                    rotation: trackCourse,
                    opacity: opacity
                })
            });
            break;
        }
        case "Fishing": {
            trackMarkerStyle = new ol.style.Style({
                image: new ol.style.Icon({
                    src: 'assets/images/marker.webp',
                    offsetOrigin: 'top-right',
                    offset: [0, 175],
                    size: [30, 30],
                    rotation: trackCourse,
                    opacity: opacity
                })
            });
            break;
        }
        case "Pleasure Craft": {
            trackMarkerStyle = new ol.style.Style({
                image: new ol.style.Icon({
                    src: 'assets/images/marker.webp',
                    offsetOrigin: 'top-right',
                    offset: [25, 175],
                    size: [30, 30],
                    rotation: trackCourse,
                    opacity: opacity
                })
            });
            break;
        }
        case "Navigation Aids": {
            break;
        }
        case "Unspecified Ships": {
            trackMarkerStyle = new ol.style.Style({
                image: new ol.style.Icon({
                    src: 'assets/images/marker.webp',
                    offsetOrigin: 'top-right',
                    offset: [95, 175],
                    size: [30, 30],
                    rotation: trackCourse,
                    opacity: opacity
                })
            });
            break;
        }
        default: {
            trackMarkerStyle = new ol.style.Style({
                image: new ol.style.Icon({
                    src: 'assets/images/marker.webp',
                    offsetOrigin: 'top-right',
                    offset: [95, 175],
                    size: [30, 30],
                    rotation: trackCourse,
                    opacity: opacity
                })
            });
        }
    }
    return trackMarkerStyle;
}

function setActiveOption(option) {
    var sketch;
    switch (option) {
        case "select": {
            zoomType = "";
            $(".map").css("cursor", "pointer");
            $(".selected").removeClass("selected");
            $(".kt-menu__item #select").addClass("selected");
            map.removeInteraction(draw);
            map.removeInteraction(select);
            map.removeOverlay(helpTooltip);
            $('#DrawingModal').modal('hide');

            select = new ol.interaction.Select({ layers: [drawingVectorLayer] });
            map.addInteraction(select);

            select.getFeatures().on('add', function (event) {
                var properties = event.element.getProperties();
                //selectedFeatureID = properties.id;
            });
            break;
        }
        case "pan": {
            zoomType = "";
            $(".map").css("cursor", "grab");
            $(".selected").removeClass("selected");
            $(".kt-menu__item #pan").addClass("selected");
            map.removeInteraction(draw);
            map.removeInteraction(select);
            map.removeOverlay(helpTooltip);
            $('#DrawingModal').modal('hide');
            break;
        }
        case "zoomin": {
            zoomType = "zoom-in";
            $(".map").css("cursor", "zoom-in");
            $(".selected").removeClass("selected");
            $(".kt-menu__item #zoomin").addClass("selected");
            map.removeInteraction(draw);
            map.removeInteraction(select);
            map.removeOverlay(helpTooltip);
            $('#DrawingModal').modal('hide');

            //map.on("click", function (evt) {
            //    if (zoomType === "zoom-in") {
            //        view = map.getView();
            //        zoom = view.getZoom();
            //        view.animate({
            //            zoom: zoom + 1,
            //            duration: 100
            //        });
            //    }

            //});
            break;
        }
        case "zoomout": {
            zoomType = "zoom-out";
            $(".map").css("cursor", "zoom-out");
            $(".selected").removeClass("selected");
            $(".kt-menu__item #zoomout").addClass("selected");
            map.removeInteraction(draw);
            map.removeInteraction(select);
            map.removeOverlay(helpTooltip);
            $('#DrawingModal').modal('hide');

            //map.on("click", function (evt) {
            //    if (zoomType === "zoom-out") {
            //        view = map.getView();
            //        zoom = view.getZoom();
            //        view.animate({
            //            zoom: zoom - 1,
            //            duration: 100
            //        });
            //    }

            //});
            break;
        }
        case "area": {
            zoomType = "";
            $(".map").css("cursor", "default");
            $(".selected").removeClass("selected");
            $(".kt-menu__item #area").addClass("selected");
            map.removeInteraction(draw);
            map.removeInteraction(select);
            map.removeOverlay(helpTooltip);
            $('#DrawingModal').modal('hide');

            var continuePolygonMsg = "Click to continue drawing the polygon";

            pointerMoveHandler = function (evt) {
                if (evt.dragging) {
                    return;
                }
                helpMsg = "Click to start measure area";
                if (sketch) {
                    geom = sketch.getGeometry();
                    if (geom instanceof ol.geom.Polygon) {
                        helpMsg = continuePolygonMsg;
                    }
                }
                helpTooltipElement.innerHTML = helpMsg;
                helpTooltip.setPosition(evt.coordinate);
                helpTooltipElement.classList.remove("hidden");
            };

            map.on("pointermove", pointerMoveHandler);

            map.getViewport().addEventListener("mouseout", function () {
                helpTooltipElement.classList.add("hidden");
            });

            formatArea = function (polygon) {
                area = ol.sphere.getArea(polygon);
                //if (area > 10000) {
                value = Math.round(area / 1000000 * 100) / 100 / 3.43;
                output = value.toFixed(3) +
                    " " + "sq.NM";
                //} else {
                //    output = Math.round(area * 100) / 100 +
                //        " " + "m<sup>2</sup>";
                //}
                return output;
            };

            function addInteraction() {
                draw = new ol.interaction.Draw({
                    source: drawingVectorSource,
                    type: 'Polygon',
                    style: new ol.style.Style({
                        fill: new ol.style.Fill({
                            color: "rgba(255, 255, 255, 0.2)"
                        }),
                        stroke: new ol.style.Stroke({
                            color: "rgba(0, 0, 0, 0.5)",
                            lineDash: [10, 10],
                            width: 2
                        }),
                        image: new ol.style.Circle({
                            radius: 5,
                            stroke: new ol.style.Stroke({
                                color: "rgba(0, 0, 0, 0.7)"
                            }),
                            fill: new ol.style.Fill({
                                color: "rgba(255, 255, 255, 0.2)"
                            })
                        })
                    })
                });

                map.addInteraction(draw);

                createMeasureTooltip();
                createHelpTooltip();

                draw.on("drawstart",
                    function (evt) {
                        sketch = evt.feature;
                        tooltipCoord = evt.coordinate;
                        listener = sketch.getGeometry().on("change", function (evt) {
                            geom = evt.target;
                            if (geom instanceof ol.geom.Polygon) {
                                output = formatArea(geom);
                                tooltipCoord = geom.getInteriorPoint().getCoordinates();
                            }
                            measureTooltipElement.innerHTML = output;
                            measureTooltip.setPosition(tooltipCoord);
                        });
                    }, this);

                draw.on("drawend",
                    function (event) {
                        measureTooltipElement.className = "tool tooltip-static";
                        measureTooltip.setOffset([0, -7]);
                        sketch = null;
                        measureTooltipElement = null;
                        createMeasureTooltip();
                        ol.Observable.unByKey(listener);
                        //featureID = featureID + 1;
                        //event.feature.setProperties({
                        //    'id': featureID
                        //});
                        zoomType = "";
                        $(".map").css("cursor", "default");
                        $(".selected").removeClass("selected");
                        map.removeInteraction(draw);
                        map.removeInteraction(select);
                        map.removeOverlay(helpTooltip);
                    }, this);
            }

            function createHelpTooltip() {
                if (helpTooltipElement) {
                    helpTooltipElement.parentNode.removeChild(helpTooltipElement);
                }
                helpTooltipElement = document.createElement("div");
                helpTooltipElement.className = "tool hidden";
                helpTooltip = new ol.Overlay({
                    element: helpTooltipElement,
                    offset: [15, 0],
                    positioning: "center-left"
                });
                map.addOverlay(helpTooltip);
            }

            function createMeasureTooltip() {
                if (measureTooltipElement) {
                    measureTooltipElement.parentNode.removeChild(measureTooltipElement);
                }
                measureTooltipElement = document.createElement("div");
                measureTooltipElement.className = "tool tooltip-measure";
                measureTooltip = new ol.Overlay({
                    element: measureTooltipElement,
                    offset: [0, -15],
                    positioning: "bottom-center"
                });
                map.addOverlay(measureTooltip);
            }

            addInteraction();
            break;
        }
        case "distance": {
            zoomType = "";
            $(".map").css("cursor", "default");
            $(".selected").removeClass("selected");
            $(".kt-menu__item #distance").addClass("selected");
            map.removeInteraction(draw);
            map.removeInteraction(select);
            map.removeOverlay(helpTooltip);
            $('#DrawingModal').modal('hide');

            var continueLineMsg = "Click to continue drawing the line";

            pointerMoveHandler = function (evt) {
                if (evt.dragging) {
                    return;
                }
                helpMsg = "Click to start measure distance";
                if (sketch) {
                    geom = sketch.getGeometry();
                    if (geom instanceof ol.geom.LineString) {
                        helpMsg = continueLineMsg;
                    }
                }
                helpTooltipElement.innerHTML = helpMsg;
                helpTooltip.setPosition(evt.coordinate);
                helpTooltipElement.classList.remove("hidden");
            };

            map.on("pointermove", pointerMoveHandler);

            map.getViewport().addEventListener("mouseout", function () {
                helpTooltipElement.classList.add("hidden");
            });

            formatLength = function (line) {
                length = ol.sphere.getLength(line);
                //if (length > 100) {
                value = Math.round(length / 1000 * 100) / 100 / 1.852;
                output = value.toFixed(3) +
                    " " + "NM";
                //} else {
                //    output = Math.round(length * 100) / 100 +
                //        " " + "m";
                //}
                return output;
            };


            function addInteraction() {
                draw = new ol.interaction.Draw({
                    source: drawingVectorSource,
                    type: 'LineString',
                    style: new ol.style.Style({
                        fill: new ol.style.Fill({
                            color: "rgba(255, 255, 255, 0.2)"
                        }),
                        stroke: new ol.style.Stroke({
                            color: "rgba(0, 0, 0, 0.5)",
                            lineDash: [10, 10],
                            width: 2
                        }),
                        image: new ol.style.Circle({
                            radius: 5,
                            stroke: new ol.style.Stroke({
                                color: "rgba(0, 0, 0, 0.7)"
                            }),
                            fill: new ol.style.Fill({
                                color: "rgba(255, 255, 255, 0.2)"
                            })
                        })
                    })
                });
                map.addInteraction(draw);

                createMeasureTooltip();
                createHelpTooltip();

                draw.on("drawstart",
                    function (evt) {
                        sketch = evt.feature;
                        tooltipCoord = evt.coordinate;
                        listener = sketch.getGeometry().on("change", function (evt) {
                            geom = evt.target;
                            if (geom instanceof ol.geom.LineString) {
                                output = formatLength(geom);
                                tooltipCoord = geom.getLastCoordinate();
                            }
                            measureTooltipElement.innerHTML = output;
                            measureTooltip.setPosition(tooltipCoord);
                        });
                    }, this);

                draw.on("drawend",
                    function (event) {
                        measureTooltipElement.className = "tool tooltip-static";
                        measureTooltip.setOffset([0, -7]);
                        sketch = null;
                        measureTooltipElement = null;
                        createMeasureTooltip();
                        ol.Observable.unByKey(listener);
                        //featureID = featureID + 1;
                        //event.feature.setProperties({
                        //    'id': featureID
                        //});
                        zoomType = "";
                        $(".map").css("cursor", "default");
                        $(".selected").removeClass("selected");
                        map.removeInteraction(draw);
                        map.removeInteraction(select);
                        map.removeOverlay(helpTooltip);
                    }, this);
            }

            function createHelpTooltip() {
                if (helpTooltipElement) {
                    helpTooltipElement.parentNode.removeChild(helpTooltipElement);
                }
                helpTooltipElement = document.createElement("div");
                helpTooltipElement.className = "tool hidden";
                helpTooltip = new ol.Overlay({
                    element: helpTooltipElement,
                    offset: [15, 0],
                    positioning: "center-left"
                });
                map.addOverlay(helpTooltip);
            }

            function createMeasureTooltip() {
                if (measureTooltipElement) {
                    measureTooltipElement.parentNode.removeChild(measureTooltipElement);
                }
                measureTooltipElement = document.createElement("div");
                measureTooltipElement.className = "tool tooltip-measure";
                measureTooltip = new ol.Overlay({
                    element: measureTooltipElement,
                    offset: [0, -15],
                    positioning: "bottom-center"
                });
                map.addOverlay(measureTooltip);
            }

            addInteraction();
            break;
        }
        case "draw": {
            zoomType = "";
            $(".map").css("cursor", "default");
            $(".selected").removeClass("selected");
            $(".kt-menu__item #draw").addClass("selected");
            map.removeInteraction(draw);
            map.removeInteraction(select);
            map.removeOverlay(helpTooltip);
            $('#DrawingModal').modal('hide');

            var placeholderElement = $('#modal-placeholder');
            $.get('Canvas?handler=Drawing').done(function (data) {
                placeholderElement.html(data);
                bindDropdowns();
                modalDraggable();
                placeholderElement.find('#DrawingModal').modal('show');
            });
            break;
        }
        case "trackList": {
            zoomType = "";
            $(".map").css("cursor", "default");
            $(".selected").removeClass("selected");
            $(".kt-menu__item #track").addClass("selected");
            map.removeInteraction(draw);
            map.removeInteraction(select);
            map.removeOverlay(helpTooltip);
            $('#DrawingModal').modal('hide');
            modalDraggable();
            $('#TracksListModal').modal('show');
            break;
        }
    }

}

function showPopupOnHover() {
    hoverContent = document.getElementById('hoverpopup-content');

    map.on('pointermove', function (evt) {
        var f = map.forEachFeatureAtPixel(evt.pixel, function (ft) {
            return ft;
        });
        if (f && f.get('type') === 'Track') {
            coordinate = evt.coordinate;
            //hoverContent.innerHTML = '<span>TN/MMSI: </span>' + f.get('trackNumber') + '<br/>' + '<span>SOURCE: </span> ' + f.get('trackType') + ' <br/>' + '<span>LAT: </span>' + f.get('lat') + '<br/>' + '<span>LON: </span>' + f.get('lon') + '<br/>' + '<span>SPEED: </span>' + f.get('speed') + '<br/>' + '<span>COURSE: </span>' + f.get('course') + '<br/>' + '<span>DESTINATION: </span>' + f.get('destination') + '<br/>' + '<span>SHIP NAME: </span>' + f.get('shipName');
            var htm = f.get('IsLloydInfoPresent') ? 'Available' : 'Not Availble';
            hoverContent.innerHTML = '<span>TN/MMSI: </span>' + f.get('trackNumber') + '<br/>' + '<span>SOURCE: </span> ' + f.get('trackType') + ' <br/>' + '<span>LAT: </span>' + f.get('lat') + '<br/>' + '<span>LON: </span>' + f.get('lon') + '<br/>' + '<span>SPEED: </span>' + f.get('speed') + '<br/>' + '<span>COURSE: </span>' + f.get('course') + '<br/>' + '<span>Lloyd Details: </span > ' + htm;
            if (typeof f.get('destination') !== "undefined")
                hoverContent.innerHTML += '<br/> <span>DESTINATION: </span>' + f.get('destination');
            if (typeof f.get('shipName') !== "undefined")
                hoverContent.innerHTML += '<br/>' + '<span>SHIP NAME: </span > ' + f.get('shipName');
                
            hoverPopupOverlay.setPosition(coordinate);
        }
        else if (f && f.get('type') === 'COIMarker') {
            coordinate = evt.coordinate;
            hoverContent.innerHTML = '<span>DATE: </span>' + f.get('reportingDatetime') + '<br />' + '<span>COINumber: </span>' + f.get('coiNumber') + '<br />' + '<span>LAT: </span>' + f.get('latitude') + '<br />' + '<span>LON: </span>' + f.get('longitude') + '<br />' + '<span>TYPE: </span>' + f.get('coiTypeName');
            hoverPopupOverlay.setPosition(coordinate);
        }
        else if (f && f.get('type') === 'PRMarker') {
            coordinate = evt.coordinate;
            hoverContent.innerHTML = '<span>DATE: </span>' + f.get('reportingDatetime') + '<br />' + '<span>PRNumber: </span>' + f.get('prNumber') + '<br />' + '<span>LAT: </span>' + f.get('latitude') + '<br />' + '<span>LON: </span>' + f.get('longitude') + '<br />' + '<span>TYPE: </span>' + f.get('coiTypeName');
            hoverPopupOverlay.setPosition(coordinate);
        }
        else if (f && f.get('type') === 'SRMarker') {
            coordinate = evt.coordinate;
            hoverContent.innerHTML = '<span>DATE: </span>' + f.get('reportingDatetime') + '<br />' + '<span>SRNumber: </span>' + f.get('srNumber') + '<br />' + '<span>PRNumber: </span>' + f.get('prNumber') + '<br />' + '<span>LAT: </span>' + f.get('latitude') + '<br />' + '<span>LON: </span>' + f.get('longitude') + '<br />' + '<span>TYPE: </span>' + f.get('coiTypeName');
            hoverPopupOverlay.setPosition(coordinate);
        }
        else if (f && f.get('type') === 'ARMarker') {
            coordinate = evt.coordinate;
            hoverContent.innerHTML = '<span>DATE: </span>' + f.get('reportingDatetime') + '<br />' + '<span>ARNumber: </span>' + f.get('arNumber') + '<br />' + '<span>LAT: </span>' + f.get('latitude') + '<br />' + '<span>LON: </span>' + f.get('longitude') + '<br />' + '<span>CLASSIFICATION: </span>' + f.get('coiClassificationName');
            hoverPopupOverlay.setPosition(coordinate);
        }
        else if (f && f.get('type') === 'LRMarker') {
            coordinate = evt.coordinate;
            hoverContent.innerHTML = '<span>DATE: </span>' + f.get('reportingDatetime') + '<br />' + '<span>LAT: </span>' + f.get('latitude') + '<br />' + '<span>LON: </span>' + f.get('longitude');
            hoverPopupOverlay.setPosition(coordinate);
        }
        else if (f && f.get('type') === 'DRMarker') {
            coordinate = evt.coordinate;
            hoverContent.innerHTML = '<span>DATE: </span>' + f.get('reportingDatetime') + '<br />' + '<span>PRNumber: </span>' + f.get('prNumber') + '<br />' + '<span>COINumber: </span>' + f.get('coiNumber') + '<br />' + '<span>LAT: </span>' + f.get('latitude') + '<br />' + '<span>LON: </span>' + f.get('longitude');
            hoverPopupOverlay.setPosition(coordinate);
        }
        else {
            hoverPopupOverlay.setPosition(undefined);
        }
    });
}

function showPopupOnSearch(feature, position) {
    var searchCloser = document.getElementById('searchpopup-closer');
    var searchContent = document.getElementById('searchpopup-content');

    searchCloser.onclick = function () {
        searchPopupOverlay.setPosition(undefined);
        searchCloser.blur();
        return false;
    };

    if (feature && feature.get('type') === 'Track') {
        searchContent.innerHTML = '<span>TN/MMSI: </span>' + feature.get('trackNumber') + '<br/>' + '<span>SOURCE: </span> ' + feature.get('trackType') + ' <br/>' + '<span>LAT: </span>' + feature.get('lat') + '<br/>' + '<span>LON: </span>' + feature.get('lon') + '<br/>' + '<span>SPEED: </span>' + feature.get('speed') + '<br/>' + '<span>COURSE: </span>' + feature.get('course');
        if (typeof feature.get('destination') !== "undefined")
            hoverContent.innerHTML += '<br/> <span>DESTINATION: </span>' + feature.get('destination');
        if (typeof feature.get('shipName') !== "undefined")
            hoverContent.innerHTML += '<br/>' + '<span>SHIP NAME: </span > ' + feature.get('shipName');
        searchPopupOverlay.setPosition(position);
    }

    else if (feature && feature.get('type') === 'COIMarker') {
        searchContent.innerHTML = '<span>DATE: </span>' + feature.get('reportingDatetime') + '<br />' + '<span>COINumber: </span>' + feature.get('coiNumber') + '<br />' + '<span>LAT: </span>' + feature.get('latitude') + '<br />' + '<span>LON: </span>' + feature.get('longitude') + '<br />' + '<span>TYPE: </span>' + feature.get('coiTypeName');
        searchPopupOverlay.setPosition(position);
    }

    else if (feature && feature.get('type') === 'PRMarker') {
        searchContent.innerHTML = '<span>DATE: </span>' + feature.get('reportingDatetime') + '<br />' + '<span>PRNumber: </span>' + feature.get('prNumber') + '<br />' + '<span>LAT: </span>' + feature.get('latitude') + '<br />' + '<span>LON: </span>' + feature.get('longitude') + '<br />' + '<span>TYPE: </span>' + feature.get('coiTypeName');
        searchPopupOverlay.setPosition(position);
    }

    else if (feature && feature.get('type') === 'SRMarker') {
        searchContent.innerHTML = '<span>DATE: </span>' + feature.get('reportingDatetime') + '<br />' + '<span>SRNumber: </span>' + feature.get('srNumber') + '<br />' + '<span>PRNumber: </span>' + feature.get('prNumber') + '<br />' + '<span>LAT: </span>' + feature.get('latitude') + '<br />' + '<span>LON: </span>' + feature.get('longitude') + '<br />' + '<span>TYPE: </span>' + feature.get('coiTypeName');
        searchPopupOverlay.setPosition(position);
    }

    else if (feature && feature.get('type') === 'LRMarker') {
        searchContent.innerHTML = '<span>DATE: </span>' + feature.get('reportingDatetime') + '<br />' + '<span>LAT: </span>' + feature.get('latitude') + '<br />' + '<span>LON: </span>' + feature.get('longitude');
        searchPopupOverlay.setPosition(position);
    }

    else if (feature && feature.get('type') === 'ARMarker') {
        searchContent.innerHTML = '<span>DATE: </span>' + feature.get('reportingDatetime') + '<br />' + '<span>ARNumber: </span>' + feature.get('arNumber') + '<br />' + '<span>LAT: </span>' + feature.get('latitude') + '<br />' + '<span>LON: </span>' + feature.get('longitude') + '<br />' + '<span>CLASSIFICATION: </span>' + feature.get('coiClassificationName');
        searchPopupOverlay.setPosition(position);
    }

    else {
        searchPopupOverlay.setPosition(undefined);
    }
}

function getDrawings(jsonString) {
    drawingFeature = (new ol.format.GeoJSON({ featureProjection: 'EPSG:3857' })).readFeature(jsonString);
    featureProperties = drawingFeature.getProperties();
    drawingFeatureStyle = new ol.style.Style({
        fill: new ol.style.Fill({
            color: featureProperties.fillColor
        }),
        stroke: new ol.style.Stroke({
            color: featureProperties.strokeColor,
            width: 2
        }),
        text: new ol.style.Text({
            text: featureProperties.name
        })
    });
    drawingFeature.setStyle(drawingFeatureStyle);
    drawingVectorSource.addFeature(drawingFeature);
}

function locate(Id, datatable) {
    if (datatable === 'PR') {
        source = prVectorSource;
    }
    if (datatable === 'SR') {
        source = srVectorSource;
    }
    if (datatable === 'COI') {
        source = coiVectorSource;
    }
    if (datatable === 'AR') {
        source = ARVectorSource;
    }
    if (datatable === 'LR') {
        source = LRVectorSource;
    }
    $('.modal').modal('hide');
    var f = source.getFeatureById(Id);
    var p = f.getGeometry().getFirstCoordinate();
    map.getView().animate({ center: p });
    showPopupOnSearch(f, p);
}

//#endregion OpenLayers

//#region Functions

function addToTrackList(trackNumber, lat, lon, speed, course, source) {
    var filteredData = tracklistDt
        .rows()
        .indexes()
        .filter(function (value, index) {
            return tracklistDt.row(value).data()[0] === trackNumber;
        });
    tracklistDt.rows(filteredData).remove().draw(false);
    tracklistDt.row.add([trackNumber, lat, lon, speed, course, source]).draw(false);
}

function openURLWindow() {
    var url = $('#url').val();
    var w = 1000;
    var h = 800;
    var left = (screen.width / 2) - (w / 2);
    var top = (screen.height / 2) - (h / 2);
    var myWindow = window.open(url, '', 'width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
    myWindow.focus();
    return false;
}

function openNewsWindow(url) {
    var w = 1000;
    var h = 800;
    var left = (screen.width / 2) - (w / 2);
    var top = (screen.height / 2) - (h / 2);
    var myWindow = window.open(url, '', 'width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
    myWindow.focus();
    return false;
}

function openSkypeWindow() {
    //var url = $('#url').val();
    var w = 1000;
    var h = 800;
    var left = (screen.width / 2) - (w / 2);
    var top = (screen.height / 2) - (h / 2);
    var myWindow = window.open('https://web.skype.com', '', 'width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
    myWindow.focus();
    return false;
}

function openPurpleFinder() {
    var w = 1000;
    var h = 800;
    var left = (screen.width / 2) - (w / 2);
    var top = (screen.height / 2) - (h / 2);
    var myWindow = window.open('https://www.purplefinder.com/', '', 'width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
    myWindow.focus();
    return false;
}

function openMarineTraffic() {
    var w = 1000;
    var h = 800;
    var left = (screen.width / 2) - (w / 2);
    var top = (screen.height / 2) - (h / 2);
    var myWindow = window.open('https://www.marinetraffic.com/en/ais/home/centerx:-12.1/centery:24.9/zoom:4', '', 'width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
    myWindow.focus();
    return false;
}

function bindDropdowns() {
    if (typeof $("#drpSubsList").val() !== 'undefined' || typeof $("#drpMultiAct").val() !== 'undefined' || typeof $("#drpMultiInfo").val() !== 'undefined') {
        $.ajax({
            url: 'Canvas?handler=AllStakeholders',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                var drpSubscriber = $("#drpSubsList");
                var drpActionAddressee = $("#drpMultiAct");
                var drpInfoAddressee = $("#drpMultiInfo");
                var loggedInSubs = $("#loggedInSubsId").val();
                drpSubscriber.empty();
                drpActionAddressee.empty();
                drpInfoAddressee.empty();
                $('#drpSubsList').prepend('<option value="">Select an option</option>');
                $.each(response, function (i, item) {
                    $('#drpSubsList').append('<option value="' + item.subscriberId + '">' + item.subscriberCode + '</option>');
                    $('#drpMultiAct').append('<option value="' + item.subscriberId + '">' + item.subscriberCode + '</option>');
                    $('#drpMultiInfo').append('<option value="' + item.subscriberId + '">' + item.subscriberName + '-' + item.subscriberCode + '</option>');
                    $("#drpMultiInfo option[value=1]").remove();
                    $("#drpMultiInfo option[value= " + loggedInSubs + " ]").remove();
                });

                var hdnSubscriber = $("#hdnSubscriberId");
                if (typeof hdnSubscriber.val() !== 'undefined' && hdnSubscriber.val() !== null) {
                    drpSubscriber.val(hdnSubscriber.val());
                }

                var hdnActionAddressee = $("#hdnActionAddresse");
                if (typeof hdnActionAddressee.val() !== 'undefined' && hdnActionAddressee.val() !== null) {
                    var actionAddresseeArray = hdnActionAddressee.val().split(",");
                    $.each(actionAddresseeArray, function (i) {
                        $("#drpMultiAct option[value='" + actionAddresseeArray[i] + "']").prop("selected", true);
                    });
                }

                var hdnInfoAddressee = $("#hdnInfoAddresse");
                if (typeof hdnInfoAddressee.val() !== 'undefined' && hdnInfoAddressee.val() !== null) {
                    var infoAddresseeArray = hdnInfoAddressee.val().split(",");
                    $.each(infoAddresseeArray, function (i) {
                        $("#drpMultiInfo option[value='" + infoAddresseeArray[i] + "']").prop("selected", true);
                    });
                }
            }
        });
    }

    if (typeof $("#drpCOITypes").val() !== 'undefined') {
        $.ajax({
            url: 'Canvas?handler=COITypes',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                var dropdown = $("#drpCOITypes");
                dropdown.empty();
                $.each(response, function (i, item) {
                    $('#drpCOITypes').append('<option value="' + item.coiTypeId + '">' + item.coiTypeName + '</option>');
                });
                $('#drpCOITypes').prepend('<option value="">Select an option</option>');
                var hdnCOIType = $("#hdnCOIType");
                if (hdnCOIType !== undefined && hdnCOIType !== null) {
                    dropdown.val(hdnCOIType.val());
                }
            }
        });
    }

    if (typeof $("#drpNatureOfThreats").val() !== 'undefined') {
        $.ajax({
            url: 'Canvas?handler=AllNatureOfThreats',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                var dropdown = $("#drpNatureOfThreats");
                dropdown.empty();
                $.each(response, function (i, item) {
                    $('#drpNatureOfThreats').append('<option value="' + item.threatId + '">' + item.threatName + '</option>');
                });
                $('#drpNatureOfThreats').prepend('<option value="">Select an option</option>');
                var hdnThreat = $("#hdnThreat");
                if (hdnThreat !== undefined && hdnThreat !== null) {
                    dropdown.val(hdnThreat.val());
                }
            }
        });
    }

    if (typeof $("#drpRoleList").val() !== 'undefined') {
        $.ajax({
            url: 'Canvas?handler=AllRoles',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                var dropdown = $("#drpRoleList");
                dropdown.empty();
                $('#drpRoleList').prepend('<option value="">Select an option</option>');
                $.each(response, function (i, item) {
                    $('#drpRoleList').append('<option value="' + item.name + '">' + item.name + '</option>');
                });
            }
        });
    }

    if (typeof $("#shapetype").val() !== 'undefined') {
        $.ajax({
            url: 'Canvas?handler=AllDrawingShapes',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                var dropdown = $("#shapetype");
                dropdown.empty();
                $('#shapetype').prepend('<option value="">Select an option</option>');
                $.each(response, function (i, item) {
                    $('#shapetype').append('<option value="' + item.shapeId + '">' + item.shapeName + '</option>');
                });
            }
        });
    }

    if (typeof $("#circleUnit").val() !== 'undefined') {
        $.ajax({
            url: 'Canvas?handler=AllRadiusUnits',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                var dropdown = $("#circleUnit");
                dropdown.empty();
                $('#circleUnit').prepend('<option value="">Select an option</option>');
                $.each(response, function (i, item) {
                    if (item.radUnitName === 'm')
                        $('#circleUnit').append('<option value="' + item.radUnitId + '" selected>' + item.radUnitName + '</option>');
                    else
                        $('#circleUnit').append('<option value="' + item.radUnitId + '">' + item.radUnitName + '</option>');
                });
            }
        });
    }

    if (typeof $("#drpNewsFeedTypeList").val() !== 'undefined') {
        $.ajax({
            url: 'Canvas?handler=AllNewsFeedTypes',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                var dropdown = $("#drpNewsFeedTypeList");
                dropdown.empty();
                $('#drpNewsFeedTypeList').prepend('<option value="">Select an option</option>');
                $.each(response, function (i, item) {
                    $('#drpNewsFeedTypeList').append('<option value="' + item.newsFeedTypeId + '">' + item.newsFeedTypeName + '</option>');
                });
            }
        });
    }

    if (typeof $("#drpConfiLvlTypes").val() !== 'undefined') {
        $.ajax({
            url: 'Canvas?handler=AllInfoConLevels',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                var dropdown = $("#drpConfiLvlTypes");
                dropdown.empty();
                $('#drpConfiLvlTypes').prepend('<option value="">Select an option</option>');
                $.each(response, function (i, item) {
                    $('#drpConfiLvlTypes').append('<option value="' + item.infoConfidenceLevelId + '">' + item.infoConfidenceLevelName + '</option>');
                });
            }
        });
    }

    if (typeof $("#drpTemplateTypeList").val() !== 'undefined') {
        $.ajax({
            url: 'Canvas?handler=AllTemplateTypes',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                var dropdown = $("#drpTemplateTypeList");
                dropdown.empty();
                $('#drpTemplateTypeList').prepend('<option value="">Select an option</option>');
                $.each(response, function (i, item) {
                    $('#drpTemplateTypeList').append('<option value="' + item.templateTypeId + '">' + item.templateTypeName + '</option>');
                });
                var hdnTemplateType = $("#hdnTemplateType");
                if (hdnTemplateType !== undefined && hdnTemplateType !== null) {
                    dropdown.val(hdnTemplateType.val());
                }
            }
        });
    }

    if (typeof $("#coiStatus").val() !== 'undefined') {
        $.ajax({
            url: 'Canvas?handler=AllCOIStatuses',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                console.log(response);
                var dropdown = $("#coiStatus");
                dropdown.empty();
                $('#coiStatus').prepend('<option value="">Select an option</option>');
                $.each(response, function (i, item) {
                    $('#coiStatus').append('<option value="' + item.coiStatusId + '">' + item.coIstatus + '</option>');
                });
                var hdnCOIStatus = $("#hdnCOIStatus");
                if (hdnCOIStatus !== undefined && hdnCOIStatus !== null) {
                    dropdown.val(hdnCOIStatus.val());
                }
            }
        });
    }
}

function modalDraggable() {
    if (!($('.modal.in').length)) {
        $('.modal-dialog-grid').css({
            'top': '0px',
            'left': 'calc(60%)',
            'margin-left': '-50%'
        });
    }
    $('.dragModal').modal({
        backdrop: false,
        show: false
    });
    $('.drag-modal-dialog').draggable({
        handle: ".drag-modal-header"
    });
}

function markRead(Id) {
    fetch('Canvas?handler=HideNotification&Id=' + Id, {
        method: 'post',
        body: new URLSearchParams(),
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    }).then(function (response) {
        if (response.status === 200) {
            count--;
            if (count > 0) {
                $('#noti_badge').text(count);
                $('#notiItem-' + Id).hide();
            }
            else {
                $('#noti_badge').hide();
                $('#notiItem-' + Id).hide();
            }
        }
    });
    return false;
    //event.preventDefault();
}

//#endregion Functions

//#region Datatables 

function prReportsDatatable() {

    prGridmodal = $('#PRGridModal');
    prDatatable = $('#modal_datatable_prReports').KTDatatable({

        data: {
            type: 'remote',
            source: {
                read: {
                    method: 'GET',
                    url: 'Canvas?handler=AllPagedPRs',
                    map: function (raw) {
                        // sample data mapping
                        var dataSet = raw;
                        if (typeof raw.data !== 'undefined') {
                            dataSet = raw.data;
                        }
                        return dataSet;
                    }
                }
            },
            pageSize: 10,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true
        },

        // layout definition
        layout: {
            scroll: true, // enable/disable datatable scroll both horizontal and vertical when needed.
            height: 500, // datatable's body's fixed height
            minHeight: 500,
            footer: false // display/hide footer
        },

        // column sorting
        sortable: true,

        pagination: true,

        search: {
            input: prGridmodal.find('#generalSearch')
        },

        // columns definition
        columns: [
            {
                field: 'coiId',
                title: 'Information',
                width: 290,
                template: function (row) {
                    var threat = {
                        'High': { 'title': 'High', 'class': 'high' },
                        'Medium': { 'title': 'Medium', 'class': 'medium' },
                        'Normal': { 'title': 'Normal', 'class': 'normal' }
                    };
                    return (row.coiNumber !== null ? '<span style="display:block;font-weight:500;">COI Number: <span style="font-weight:normal">' + row.coiNumber + '</span></span>' : '') +
                        (row.prNumber !== null ? '<span style="display:block;font-weight:500;">PR Number: <span style="font-weight:normal">' + row.prNumber + '</span></span>' : '') +
                        (row.reportingDatetime !== null ? '<span style="display:block;font-weight:500;">Reported DateTime: <span style="font-weight:normal">' + row.reportingDatetime + '</span></span>' : '') +
                        (row.coiTypeName !== null ? '<span style="display:block;font-weight:500;">Type: <span style="font-weight:normal">' + row.coiTypeName + '</span></span>' : '') +
                        (row.threatName !== null ? '<span style="font-weight:500;">Threat: </span><span class="kt-badge ' + threat[row.threatName].class + ' kt-badge--inline kt-badge--pill">' + threat[row.threatName].title + '</span>' : '');
                }
            },
            {
                field: 'mmsi',
                title: 'Position Information',
                template: function (row) {
                    return (row.latitude !== null ? '<span style="display:block;font-weight:500;">Latitude: <span style="font-weight:normal">' + row.latitude + '</span></span>' : '') +
                        (row.longitude !== null ? '<span style="display:block;font-weight:500;">Longitude: <span style="font-weight:normal">' + row.longitude + '</span></span>' : '') +
                        (row.mmsi !== null ? '<span style="display:block;font-weight:500;">MMSI: <span style="font-weight:normal">' + row.mmsi + '</span></span>' : '') +
                        (row.course !== null ? '<span style="display:block;font-weight:500;">Course: <span style="font-weight:normal">' + row.course + '</span></span>' : '') +
                        (row.speed !== null ? '<span style="display:block;font-weight:500;">Speed: <span style="font-weight:normal">' + row.speed + '</span></span>' : '');
                }
            },
            {
                field: 'actionAddressee',
                title: 'Addressee',
                template: function (row) {
                    return (row.actionAddresseeCodes !== null ? '<span style="display:block;font-weight:500;">Action Addressee: <span style="font-weight:normal">' + row.actionAddresseeCodes + '</span></span>' : '') +
                        (row.informationAddresseeCodes !== null ? '<span style="display:block;font-weight:500;">Info Addressee: <span style="font-weight:normal">' + row.informationAddresseeCodes + '</span></span>' : '');
                }
            },
            {
                field: 'lastObservationDatetime',
                title: 'Last Observation',
                template: function (row) {
                    return (row.lastObservationDatetime !== null ? '<span style="font-weight:normal">' + row.lastObservationDatetime + '</span>' : '');
                }
            },
            {
                field: 'remarks',
                title: 'Remarks'
            },
            {
                field: 'subscriberCode',
                width: 80,
                title: 'Initiator'
            },
            {
                field: 'Actions',
                title: 'Actions',
                sortable: false,
                width: 70,
                overflow: 'visible',
                autoHide: false,
                template: function (row) {
                    var datatable = 'PR';
                    return "\
						<div class='dropdown'>\
							<a href='javascript:;' class='btn btn-sm btn-clean btn-icon btn-icon-md' data-toggle='dropdown'>\
                                <i class='la la-ellipsis-h'></i>\
                            </a>\
						  	<div class='dropdown-menu dropdown-menu-right'>\
                                <a href='javascript:;' onclick=\"viewPR(" + row.id + ",'" + row.mmsi + "','" + row.course + "','" + row.speed + "')\" class='dropdown-item'><i class='la la-edit'></i> View Report</a>\
						    	<a href='javascript:;' onclick=\"showSRReport("+ row.id + ",'" + row.mmsi + "','" + row.course + "','" + row.speed + "','" + row.coiNumber + "','" + row.isLost + "','" + row.isDropped + "','" + row.subscriberCode + "')\" class='dropdown-item'><i class='la la-edit'></i> Subsequent Report</a>\
						    	<a href='javascript:;' onclick=\"showLRReportPR("+ row.id + ",'" + row.mmsi + "','" + row.course + "','" + row.speed + "','" + row.coiNumber + "','" + row.isLost + "','" + row.isDropped + "')\" class='dropdown-item'><i class='la la-leaf'></i> Lost Report</a>\
						    	<a href='javascript:;' onclick=\"showDRReportPR("+ row.id + ",'" + row.coiNumber + "'," + row.isDropped + ")\" class='dropdown-item'><i class='la la-print'></i> Drop Report</a>\
						    	<a href='javascript:;' id='makeCOI' onclick=\"showCOIActReportPR("+ row.id + ",'" + row.mmsi + "','" + row.course + "','" + row.speed + "','" + row.coiNumber + "','" + row.isLost + "','" + row.isDropped + "')\" class='dropdown-item'><i class='la la-print'></i> Make COI</a>\
						  	</div >\
						</div>\
						<a href='javascript:;' onclick=\"locate('"+ row.prNumber + "', '" + datatable + "')\" class='btn btn-sm btn-clean btn-icon btn-icon-md' title='Locate'>\
							<i class='fa fa-search-location'></i>\
						</a>\
					";
                }
            }
        ]
    });


    prGridmodal.find('#kt_form_threat').on('change', function () {
        prDatatable.search($(this).val().toLowerCase(), 'threatName');
    });

    prGridmodal.find('#kt_form_threat').selectpicker();

}

function subsReportsDatatable() {

    srGridModal = $('#SRGridModal');
    srDatatable = $('#modal_datatable_subsReports').KTDatatable({
        // datasource definition
        data: {
            type: 'remote',
            source: {
                read: {
                    method: 'GET',
                    url: 'Canvas?handler=AllPagedSRs',
                    map: function (raw) {
                        // sample data mapping
                        var dataSet = raw;
                        if (typeof raw.data !== 'undefined') {
                            dataSet = raw.data;
                        }
                        return dataSet;
                    }
                }
            },
            pageSize: 10,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true
        },

        // layout definition
        layout: {
            scroll: true, // enable/disable datatable scroll both horizontal and vertical when needed.
            height: 500, // datatable's body's fixed height
            minHeight: 500,
            footer: false // display/hide footer
        },

        // column sorting
        sortable: true,

        pagination: true,

        search: {
            input: srGridModal.find('#generalSearch')
        },

        // columns definition
        columns: [
            {
                field: 'coiId',
                title: 'Information',
                width: 290,
                template: function (row) {
                    var threat = {
                        'High': { 'title': 'High Alert', 'class': 'high' },
                        'Medium': { 'title': 'Medium', 'class': 'medium' },
                        'Normal': { 'title': 'Normal', 'class': 'normal' }
                    };
                    return (row.coiNumber !== null ? '<span style="display:block;font-weight:500;">COI Number: <span style="font-weight:normal">' + row.coiNumber + '</span></span>' : '') +
                        (row.prNumber !== null ? '<span style="display:block;font-weight:500;">PR Number: <span style="font-weight:normal">' + row.prNumber + '</span></span>' : '') +
                        (row.srNumber !== null ? '<span style="display:block;font-weight:500;">SR Number: <span style="font-weight:normal">' + row.srNumber + '</span></span>' : '') +
                        (row.reportingDatetime !== null ? '<span style="display:block;font-weight:500;">Reported DateTime: <span style="font-weight:normal">' + row.reportingDatetime + '</span></span>' : '') +
                        (row.coiTypeName !== null ? '<span style="display:block;font-weight:500;">Type: <span style="font-weight:normal">' + row.coiTypeName + '</span></span>' : '') +
                        (row.threatName !== null ? '<span style="font-weight:500;">Threat: </span><span class="kt-badge ' + threat[row.threatName].class + ' kt-badge--inline kt-badge--pill">' + threat[row.threatName].title + '</span>' : '') +
                        (row.infoConfidenceLevelId !== null ? '<span style="display:block;font-weight:500;">Info Confidence Level: <span style="font-weight:normal">' + row.infoConfidenceLevelName + '</span></span>' : '');
                }
            },
            {
                field: 'mmsi',
                title: 'Position Information',
                template: function (row) {
                    return (row.latitude !== null ? '<span style="display:block;font-weight:500;">Latitude: <span style="font-weight:normal">' + row.latitude + '</span></span>' : '') +
                        (row.longitude !== null ? '<span style="display:block;font-weight:500;">Longitude: <span style="font-weight:normal">' + row.longitude + '</span></span>' : '') +
                        (row.mmsi !== null ? '<span style="display:block;font-weight:500;">MMSI: <span style="font-weight:normal">' + row.mmsi + '</span></span>' : '') +
                        (row.course !== null ? '<span style="display:block;font-weight:500;">Course: <span style="font-weight:normal">' + row.course + '</span></span>' : '') +
                        (row.speed !== null ? '<span style="display:block;font-weight:500;">Speed: <span style="font-weight:normal">' + row.speed + '</span></span>' : '');
                }
            },
            {
                field: 'actionAddressee',
                title: 'Addressee',
                template: function (row) {
                    return (row.actionAddresseeCodes !== null ? '<span style="display:block;font-weight:500;">Action Addressee: <span style="font-weight:normal">' + row.actionAddresseeCodes + '</span></span>' : '') +
                        (row.informationAddresseeCodes !== null ? '<span style="display:block;font-weight:500;">Info Addressee: <span style="font-weight:normal">' + row.informationAddresseeCodes + '</span></span>' : '');
                }
            },
            {
                field: 'lastObservationDatetime',
                title: 'Last Observation'
            },
            {
                field: 'remarks',
                title: 'Remarks'
            },
            {
                field: 'subscriberCode',
                width: 80,
                title: 'Initiator'
            },
            {
                field: 'Actions',
                title: 'Actions',
                sortable: false,
                width: 70,
                overflow: 'visible',
                autoHide: false,
                template: function (row) {
                    var datatable = 'SR';
                    return "\
                    <div class='dropdown'>\
                        <a href='javascript:;' class='btn btn-sm btn-clean btn-icon btn-icon-md m-0' data-toggle='dropdown'>\
                           <i class='la la-ellipsis-h'></i>\
                        </a>\
                        <div class='dropdown-menu dropdown-menu-right'>\
                            <a href='javascript:;' onclick=\"viewSR("+ row.id + ",'" + row.mmsi + "','" + row.course + "','" + row.speed + "')\" class='dropdown-item'><i class='la la-edit'></i> View Report</a>\
                            <a href='javascript:;' class='dropdown-item' onclick=\"showCOIActReportSR("+ row.id + ",'" + row.mmsi + "','" + row.course + "','" + row.speed + "','" + row.coiNumber + "')\"><i class='la la-edit'></i>Make COI</a>\
                        </div>\
                    </div>\
                    <a href='javascript:;' onclick=\"locate('"+ row.srNumber + "','" + datatable + "')\" class='btn btn-sm btn-clean btn-icon btn-icon-md' title='Locate'>\
							<i class='fa fa-search-location'></i>\
					</a>\
                    ";
                }
            }]
    });


    srGridModal.find('#kt_form_threat').on('change', function () {
        srDatatable.search($(this).val().toLowerCase(), 'threatName');
    });

    srGridModal.find('#kt_form_threat').selectpicker();

}

function coiReportsDatatable() {

    coiGridmodal = $('#COIGridModal');
    coiDatatable = $('#modal_datatable_coiReports').KTDatatable({
        // datasource definition
        data: {
            type: 'remote',
            source: {
                read: {
                    method: 'GET',
                    url: 'Canvas?handler=AllPagedCOIs',
                    map: function (raw) {
                        // sample data mapping
                        var dataSet = raw;
                        if (typeof raw.data !== 'undefined') {
                            dataSet = raw.data;
                        }
                        return dataSet;
                    }
                }
            },
            pageSize: 10,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true
        },

        // layout definition
        layout: {
            scroll: true, // enable/disable datatable scroll both horizontal and vertical when needed.
            height: 500, // datatable's body's fixed height
            minHeight: 500,
            footer: false // display/hide footer
        },

        // column sorting
        sortable: true,

        pagination: true,

        search: {
            input: coiGridmodal.find('#generalSearch')
        },

        // columns definition
        columns: [
            {
                field: 'coiId',
                title: 'Information',
                width: 290,
                template: function (row) {
                    var threat = {
                        'High': { 'title': 'High Alert', 'class': 'high' },
                        'Medium': { 'title': 'Medium', 'class': 'medium' },
                        'Normal': { 'title': 'Normal', 'class': 'normal' }
                    };
                    return (row.coiNumber !== null ? '<span style="display:block;font-weight:500;">COI Number: <span style="font-weight:normal">' + row.coiNumber + '</span></span>' : '') +
                        (row.prNumber !== null ? '<span style="display:block;font-weight:500;">PR Number: <span style="font-weight:normal">' + row.prNumber + '</span></span>' : '') +
                        (row.reportingDatetime !== null ? '<span style="display:block;font-weight:500;">Reported DateTime: <span style="font-weight:normal">' + row.reportingDatetime + '</span></span>' : '') +
                        (row.coiTypeName !== null ? '<span style="display:block;font-weight:500;">Type: <span style="font-weight:normal">' + row.coiTypeName + '</span></span>' : '') +
                        (row.threatName !== null ? '<span style="font-weight:500;">Threat: </span><span class="kt-badge ' + threat[row.threatName].class + ' kt-badge--inline kt-badge--pill">' + threat[row.threatName].title + '</span>' : '');
                }
            },
            {
                field: 'mmsi',
                title: 'Position Information',
                template: function (row) {
                    return (row.latitude !== null ? '<span style="display:block;font-weight:500;">Latitude: <span style="font-weight:normal">' + row.latitude + '</span></span>' : '') +
                        (row.longitude !== null ? '<span style="display:block;font-weight:500;">Longitude: <span style="font-weight:normal">' + row.longitude + '</span></span>' : '') +
                        (row.mmsi !== null ? '<span style="display:block;font-weight:500;">MMSI: <span style="font-weight:normal">' + row.mmsi + '</span></span>' : '') +
                        (row.course !== null ? '<span style="display:block;font-weight:500;">Course: <span style="font-weight:normal">' + row.course + '</span></span>' : '') +
                        (row.speed !== null ? '<span style="display:block;font-weight:500;">Speed: <span style="font-weight:normal">' + row.speed + '</span></span>' : '');
                }
            },
            {
                field: 'actionAddressee',
                title: 'Addressee',
                template: function (row) {
                    return '<span style="display:block;font-weight:500;">Action Addressee: <span style="font-weight:normal">' + row.actionAddresseeCodes + '</span></span>\
                            <span style="display:block;font-weight:500;">Info Addressee: <span style="font-weight:normal">' + row.informationAddresseeCodes + '</span></span>';
                }
            },
            {
                field: 'lastObservationDatetime',
                title: 'Last Observation'
            },
            {
                field: 'remarks',
                title: 'Remarks'
            },
            {
                field: 'subscriberCode',
                width: 80,
                title: 'Initiator'
            },
            {
                field: 'Actions',
                title: 'Actions',
                sortable: false,
                width: 70,
                overflow: 'visible',
                autoHide: false,
                template: function (row) {
                    var datatable = 'COI';
                    return "\
						<div class='dropdown'>\
							<a href='javascript:;' class='btn btn-sm btn-clean btn-icon btn-icon-md' data-toggle='dropdown'>\
                                <i class='la la-ellipsis-h'></i>\
                            </a>\
						  	<div class='dropdown-menu dropdown-menu-right'>\
                                <a href='javascript:;' onclick=\"viewCOI("+ row.coiId + ",'" + row.mmsi + "','" + row.course + "','" + row.speed + "')\" class='dropdown-item'><i class='la la-edit'></i>View Report</a>\
						    	<a href='javascript:;' onclick=\"showAmpReport("+ row.coiId + ",'" + row.mmsi + "','" + row.course + "','" + row.speed + "','" + row.isLost + "','" + row.isDropped + "')\" class='dropdown-item'><i class='la la-edit'></i>Amplifying Report</a>\
						    	<a href='javascript:;' onclick=\"showLRReportCOI("+ row.coiId + ",'" + row.mmsi + "','" + row.course + "','" + row.speed + "','" + row.isLost + "','" + row.isDropped + "')\" class='dropdown-item'><i class='la la-leaf'></i>Lost Report</a>\
						    	<a href='javascript:;' onclick=\"showDRReportCOI("+ row.coiId + ",'" + row.isDropped + "')\" class='dropdown-item'><i class='la la-print'></i>Drop Report</a>\
						  	</div >\
						</div>\
                        <a href='javascript:;' onclick=\"locate('"+ row.coiNumber + "', '" + datatable + "')\" class='btn btn-sm btn-clean btn-icon btn-icon-md' title='Locate'>\
							<i class='fa fa-search-location'></i>\
					    </a>\
					";
                }
            }
        ]
    });


    coiGridmodal.find('#kt_form_threat').on('change', function () {
        coiDatatable.search($(this).val().toLowerCase(), 'threatName');
    });

    coiGridmodal.find('#kt_form_type').on('change', function () {
        coiDatatable.search($(this).val().toLowerCase(), 'type');
    });

    coiGridmodal.find('#kt_form_threat,#kt_form_type').selectpicker();

}

function ampReportsDatatable() {

    ARGridModal = $('#ARGridModal');
    ARDatatable = $('#modal_datatable_ampReports').KTDatatable({
        // datasource definition
        data: {
            type: 'remote',
            source: {
                read: {
                    method: 'GET',
                    url: 'Canvas?handler=AllPagedARs',
                    map: function (raw) {
                        // sample data mapping
                        var dataSet = raw;
                        if (typeof raw.data !== 'undefined') {
                            dataSet = raw.data;
                        }
                        return dataSet;
                    }
                }
            },
            pageSize: 10,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true
        },

        // layout definition
        layout: {
            scroll: true, // enable/disable datatable scroll both horizontal and vertical when needed.
            height: 500, // datatable's body's fixed height
            minHeight: 500,
            footer: false // display/hide footer
        },

        // column sorting
        sortable: true,

        pagination: true,

        search: {
            input: ARGridModal.find('#generalSearch')
        },

        // columns definition
        columns: [
            {
                field: 'coiId',
                title: 'Information',
                width: 290,
                template: function (row) {
                    return (row.arNumber !== null ? '<span style="display:block;font-weight:500;">AR Number: <span style="font-weight:normal">' + row.arNumber + '</span></span>' : '') +
                        (row.reportingDatetime !== null ? '<span style="display:block;font-weight:500;">Reported DateTime: <span style="font-weight:normal">' + row.reportingDatetime + '</span></span>' : '') +
                        (row.coiClassificationName !== null ? '<span style="display:block;font-weight:500;">Classification: <span style="font-weight:normal">' + row.coiClassificationName + '</span></span>' : '') +
                        (row.proposedCOIResCategory !== null ? '<span style="display:block;font-weight:500;">Response Category: <span style="font-weight:normal">' + row.proposedCOIResCategory + '</span></span>' : '') +
                        (row.proposedDesiredEndState !== null ? '<span style="display:block;font-weight:500;">End State: <span style="font-weight:normal">' + row.proposedDesiredEndState + '</span></span>' : '');
                }
            },
            {
                field: 'mmsi',
                title: 'Position Information',
                template: function (row) {
                    return (row.latitude !== null ? '<span style="display:block;font-weight:500;">Latitude: <span style="font-weight:normal">' + row.latitude + '</span></span>' : '') +
                        (row.longitude !== null ? '<span style="display:block;font-weight:500;">Longitude: <span style="font-weight:normal">' + row.longitude + '</span></span>' : '') +
                        (row.mmsi !== null ? '<span style="display:block;font-weight:500;">MMSI: <span style="font-weight:normal">' + row.mmsi + '</span></span>' : '') +
                        (row.course !== null ? '<span style="display:block;font-weight:500;">Course: <span style="font-weight:normal">' + row.course + '</span></span>' : '') +
                        (row.speed !== null ? '<span style="display:block;font-weight:500;">Speed: <span style="font-weight:normal">' + row.speed + '</span></span>' : '');
                }
            },
            {
                field: 'actionAddressee',
                title: 'Addressee',
                template: function (row) {
                    return (row.actionAddresseeCodes !== null ? '<span style="display:block;font-weight:500;">Action Addressee: <span style="font-weight:normal">' + row.actionAddresseeCodes + '</span></span>' : '') +
                        (row.informationAddresseeCodes !== null ? '<span style="display:block;font-weight:500;">Info Addressee: <span style="font-weight:normal">' + row.informationAddresseeCodes + '</span></span>' : '');
                }
            },
            {
                field: 'remarks',
                title: 'Remarks'
            },
            {
                field: 'subscriberCode',
                width: 80,
                title: 'Initiator'
            },
            {
                field: 'Actions',
                title: 'Actions',
                sortable: false,
                width: 70,
                overflow: 'visible',
                autoHide: false,
                template: function (row) {
                    var datatable = 'AR';
                    return '\
						<div class="dropdown">\
							<a href="javascript:;" class="btn btn-sm btn-clean btn-icon btn-icon-md" data-toggle="dropdown">\
                                <i class="la la-ellipsis-h"></i>\
                            </a>\
						  	<div class="dropdown-menu dropdown-menu-right">\
                                <a href="javascript:;" onclick="viewAR('+ row.arId + ',' + row.mmsi + ',' + row.course + ',' + row.speed + ')" class="dropdown-item"><i class="la la-edit"></i>View Report</a>\
						  	</div >\
						</div>\
                    <a href="javascript:;" onclick="locate(\'' + row.arNumber + '\',\'' + datatable + '\')" class="btn btn-sm btn-clean btn-icon btn-icon-md m-0" title="Locate">\
                        <i class="fa fa-search-location"></i>\
                    </a>\
                ';
                }
            }
        ]
    });
}

function lostReportsDatatable() {

    LRGridModal = $('#LostContactGridModal');
    LRDatatable = $('#modal_datatable_lostReports').KTDatatable({
        // datasource definition
        data: {
            type: 'remote',
            source: {
                read: {
                    method: 'GET',
                    url: 'Canvas?handler=AllPagedLRs',
                    map: function (raw) {
                        // sample data mapping
                        var dataSet = raw;
                        if (typeof raw.data !== 'undefined') {
                            dataSet = raw.data;
                        }
                        return dataSet;
                    }
                }
            },
            pageSize: 10,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true
        },

        // layout definition
        layout: {
            scroll: true, // enable/disable datatable scroll both horizontal and vertical when needed.
            height: 500, // datatable's body's fixed height
            minHeight: 500,
            footer: false, // display/hide footer
        },

        // column sorting
        sortable: true,

        pagination: true,

        search: {
            input: LRGridModal.find('#generalSearch')
        },

        // columns definition
        columns: [
            {
                field: 'coiId',
                title: 'Information',
                width: 290,
                template: function (row) {
                    return (row.coiNumber !== null ? '<span style="display:block;font-weight:500;">COI Number: <span style="font-weight:normal">' + row.coiNumber + '</span></span>' : '') +
                        (row.prNumber !== null ? '<span style="display:block;font-weight:500;">PR Number: <span style="font-weight:normal">' + row.prNumber + '</span></span>' : '') +
                        (row.reportingDatetime !== null ? '<span style="display:block;font-weight:500;">Reported DateTime: <span style="font-weight:normal">' + row.reportingDatetime + '</span></span>' : '');
                }
            },
            {
                field: 'mmsi',
                title: 'Position Information',
                template: function (row) {
                    return (row.latitude !== null ? '<span style="display:block;font-weight:500;">Latitude: <span style="font-weight:normal">' + row.latitude + '</span></span>' : '') +
                        (row.longitude !== null ? '<span style="display:block;font-weight:500;">Longitude: <span style="font-weight:normal">' + row.longitude + '</span></span>' : '') +
                        (row.mmsi !== null ? '<span style="display:block;font-weight:500;">MMSI: <span style="font-weight:normal">' + row.mmsi + '</span></span>' : '') +
                        (row.course !== null ? '<span style="display:block;font-weight:500;">Course: <span style="font-weight:normal">' + row.course + '</span></span>' : '') +
                        (row.speed !== null ? '<span style="display:block;font-weight:500;">Speed: <span style="font-weight:normal">' + row.speed + '</span></span>' : '');
                }
            },
            {
                field: 'actionAddresseeCodes',
                title: 'Addressee'
            },
            {
                field: 'lastObservationDatetime',
                title: 'Last Observation'
            },
            {
                field: 'remarks',
                title: 'Remarks'
            },
            {
                field: 'subscriberCode',
                width: 80,
                title: 'Initiator'
            },
            {
                field: 'Actions',
                title: 'Actions',
                sortable: false,
                width: 70,
                overflow: 'visible',
                autoHide: false,
                template: function (row) {
                    var datatable = 'LR';
                    return '\
						<div class="dropdown">\
							<a href="javascript:;" class="btn btn-sm btn-clean btn-icon btn-icon-md" data-toggle="dropdown">\
                                <i class="la la-ellipsis-h"></i>\
                            </a>\
						  	<div class="dropdown-menu dropdown-menu-right">\
                                <a href="javascript:;" onclick="viewLR('+ row.id + ',' + row.mmsi + ',' + row.course + ',' + row.speed + ')" class="dropdown-item"><i class="la la-edit"></i>View Report</a>\
						  	</div >\
						</div>\
                    <a href="javascript:;" onclick="locate('+ row.id + ', \'' + datatable + '\')" class="btn btn-sm btn-clean btn-icon btn-icon-md m-0" title="Locate">\
                        <i class="fa fa-search-location"></i>\
                    </a>\
                ';
                }
            }
        ]
    });
}

function dropReportsDatatable() {
    deleteRow = false;
    datatable = 'DR';
    DRGridModal = $('#DropContactGridModal');
    DRDatatable = $('#modal_datatable_dropReports').KTDatatable({
        // datasource definition
        data: {
            type: 'remote',
            source: {
                read: {
                    method: 'GET',
                    url: 'Canvas?handler=AllPagedDRs',
                    map: function (raw) {
                        // sample data mapping
                        var dataSet = raw;
                        if (typeof raw.data !== 'undefined') {
                            dataSet = raw.data;
                        }
                        return dataSet;
                    }
                }
            },
            pageSize: 10,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true
        },

        // layout definition
        layout: {
            scroll: true, // enable/disable datatable scroll both horizontal and vertical when needed.
            height: 500, // datatable's body's fixed height
            minHeight: 500,
            footer: false // display/hide footer
        },

        // column sorting
        sortable: true,

        pagination: true,

        search: {
            input: DRGridModal.find('#generalSearch')
        },

        // columns definition
        columns: [
            {
                field: 'coiId',
                title: 'Information',
                width: 290,
                template: function (row) {
                    return (row.coiNumber !== null ? '<span style="display:block;font-weight:500;">COI Number: <span style="font-weight:normal">' + row.coiNumber + '</span></span>' : '') +
                        (row.prNumber !== null ? '<span style="display:block;font-weight:500;">PR Number: <span style="font-weight:normal">' + row.prNumber + '</span></span>' : '') +
                        (row.reportingDatetime !== null ? '<span style="display:block;font-weight:500;">Reported DateTime: <span style="font-weight:normal">' + row.reportingDatetime + '</span></span>' : '');
                }
            },
            {
                field: 'coIstatus',
                title: 'Present Status of COI'
            },
            {
                field: 'actionAddresseeCodes',
                title: 'Addressee'
            },
            {
                field: 'remarks',
                title: 'Remarks'
            },
            {
                field: 'subscriberCode',
                width: 80,
                title: 'Initiator'
            },
            {
                field: 'Actions',
                title: 'Actions',
                sortable: false,
                width: 110,
                overflow: 'visible',
                autoHide: false,
                template: function (row) {
                    return "\
                    <div class='dropdown'>\
                        <a href='javascript:;' class='btn btn-sm btn-clean btn-icon btn-icon-md m-0' data-toggle='dropdown'>\
                                           <i class='la la-ellipsis-h'></i>\
                        </a>\
                        <div class='dropdown-menu dropdown-menu-right'>\
                            <a href='javascript:;' onclick=\"viewDR("+ row.id + ")\" class='dropdown-item'><i class='la la-edit'></i> View Report</a>\
                            <a class='dropdown-item' href='javascript:;' onclick=\"createAAR("+ row.id + ",'" + row.subscriberCode + "','" + row.aarCreated + "')\"><i class='la la-edit'></i> After Action Report</a>\
                        </div>\
                    </div>\
                    <a href='javascript:;' onclick=\"deleteRecord("+ row.id + ", '" + deleteRow + "','" + datatable + "')\" class='btn btn-sm btn-clean btn-icon btn-icon-md' title='Delete'>\
						<i class='la la-trash'></i>\
					</a>\
                    <a href='javascript:;' onclick=\"locatePR()\" class='btn btn-sm btn-clean btn-icon btn-icon-md' title='Locate'>\
							<i class='fa fa-search-location'></i>\
					</a>\
                 ";
                }
            }]
    });


    DRGridModal.find('#kt_form_status').on('change', function () {
        DRDatatable.search($(this).val().toLowerCase(), 'status');
    });

    DRGridModal.find('#kt_form_type').on('change', function () {
        DRDatatable.search($(this).val().toLowerCase(), 'type');
    });

    DRGridModal.find('#kt_form_status,#kt_form_type').selectpicker();
}

function AAReportsDatatable() {

    AARGridModal = $('#AARGridModal');
    AARDatatable = $('#modal_datatable_AAReports').KTDatatable({
        // datasource definition
        data: {
            type: 'remote',
            source: {
                read: {
                    method: 'GET',
                    url: 'Canvas?handler=AllPagedAARs',
                    map: function (raw) {
                        // sample data mapping
                        var dataSet = raw;
                        if (typeof raw.data !== 'undefined') {
                            dataSet = raw.data;
                        }
                        return dataSet;
                    }
                }
            },
            pageSize: 10,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true
        },

        // layout definition
        layout: {
            scroll: true, // enable/disable datatable scroll both horizontal and vertical when needed.
            height: 500, // datatable's body's fixed height
            minHeight: 500,
            footer: false // display/hide footer
        },

        // column sorting
        sortable: true,

        pagination: true,

        search: {
            input: AARGridModal.find('#generalSearch')
        },

        // columns definition
        columns: [
            {
                field: 'coiId',
                title: 'Information',
                width: 290,
                template: function (row) {
                    var threat = {
                        'High': { 'title': 'High Alert', 'class': 'high' },
                        'Medium': { 'title': 'Medium', 'class': 'medium' },
                        'Normal': { 'title': 'Normal', 'class': 'normal' }
                    };
                    return (row.reportingDatetime !== null ? '<span style="display:block;font-weight:500;">Reported DateTime: <span style="font-weight:normal">' + row.reportingDatetime + '</span></span>' : '') +
                        (row.initiationDatetime !== null ? '<span style="display:block;font-weight:500;">Initiation DateTime: <span style="font-weight:normal">' + row.initiationDatetime + '</span></span>' : '') +
                        //(row.coiActivationDatetime !== null ? '<span style="display:block;font-weight:500;">COI Activation DateTime: <span style="font-weight:normal">' + row.coiActivationDateTime + '</span></span>' : '') +
                        (row.infoConfidenceLevelName !== null ? '<span style="display:block;font-weight:500;">Confidence Level: <span style="font-weight:normal">' + row.infoConfidenceLevelName + '</span></span>' : '') +
                        (row.sourcesOfInfo !== null ? '<span style="display:block;font-weight:500;">Confidence Level: <span style="font-weight:normal">' + row.sourcesOfInfo + '</span></span>' : '') +
                        (row.coiTypeName !== null ? '<span style="display:block;font-weight:500;">Type: <span style="font-weight:normal">' + row.coiTypeName + '</span></span>' : '') +
                        (row.threatName !== null ? '<span style="font-weight:500;">Threat: </span><span class="kt-badge ' + threat[row.threatName].class + ' kt-badge--inline kt-badge--pill">' + threat[row.threatName].title + '</span>' : '') +
                        (row.addressedTo !== null ? '<span style="display:block;font-weight:500;">Addressed To: <span style="font-weight:normal">' + row.addressedTo + '</span></span>' : '');
                }
            },
            {
                field: 'initialReportedMMSI',
                title: 'Initial Position',
                template: function (row) {
                    return (row.initialReportedLatitude !== null ? '<span style="display:block;font-weight:500;">Latitude: <span style="font-weight:normal">' + row.initialReportedLatitude + '</span></span>' : '') +
                        (row.initialReportedLongitude !== null ? '<span style="display:block;font-weight:500;">Longitude: <span style="font-weight:normal">' + row.initialReportedLongitude + '</span></span>' : '') +
                        (row.initialReportedMMSI !== null ? '<span style="display:block;font-weight:500;">MMSI: <span style="font-weight:normal">' + row.initialReportedMMSI + '</span></span>' : '') +
                        (row.initialReportedCourse !== null ? '<span style="display:block;font-weight:500;">Course: <span style="font-weight:normal">' + row.initialReportedCourse + '</span></span>' : '') +
                        (row.initialReportedSpeed !== null ? '<span style="display:block;font-weight:500;">Speed: <span style="font-weight:normal">' + row.initialReportedSpeed + '</span></span>' : '');
                }
            },
            {
                field: 'lastReportedMMSI',
                title: 'Last Position',
                template: function (row) {
                    return (row.lastReportedLatitude !== null ? '<span style="display:block;font-weight:500;">Latitude: <span style="font-weight:normal">' + row.lastReportedLatitude + '</span></span>' : '') +
                        (row.lastReportedLongitude !== null ? '<span style="display:block;font-weight:500;">Longitude: <span style="font-weight:normal">' + row.lastReportedLongitude + '</span></span>' : '') +
                        (row.lastReportedMMSI !== null ? '<span style="display:block;font-weight:500;">MMSI: <span style="font-weight:normal">' + row.lastReportedMMSI + '</span></span>' : '') +
                        (row.lastReportedCourse !== null ? '<span style="display:block;font-weight:500;">Course: <span style="font-weight:normal">' + row.lastReportedCourse + '</span></span>' : '') +
                        (row.lastReportedSpeed !== null ? '<span style="display:block;font-weight:500;">Speed: <span style="font-weight:normal">' + row.lastReportedSpeed + '</span></span>' : '');
                }
            },
            {
                field: 'remarks',
                title: 'Remarks'
            },
            {
                field: 'subscriberCode',
                width: 80,
                title: 'Initiator'
            },
            {
                field: 'Actions',
                title: 'Actions',
                sortable: false,
                width: 70,
                overflow: 'visible',
                autoHide: false,
                template: function (row) {
                    return "\
                    <div class='dropdown'>\
                        <a href='javascript:;' class='btn btn-sm btn-clean btn-icon btn-icon-md m-0' data-toggle='dropdown'>\
                                           <i class='la la-ellipsis-h'></i>\
                                       </a>\
                        <div class='dropdown-menu dropdown-menu-right'>\
                            <a href='javascript:;' onclick=\"viewAAR('"+ row.aarId + "','" + row.initialReportedMMSI + "','" + row.initialReportedCourse + "','" + row.initialReportedSpeed + "','" + row.lastReportedMMSI + "','" + row.lastReportedCourse + "','" + row.lastReportedSpeed + "')\" class='dropdown-item'><i class='la la-edit'></i> View Report</a>\
                        </div>\
                    </div>\
                 ";
                }
            }
        ]
    });


    AARGridModal.find('#kt_form_threat').on('change', function () {
        AARDatatable.search($(this).val().toLowerCase(), 'threatName');
    });

    AARGridModal.find('#kt_form_threat').selectpicker();
}

function TemplateReportsDatatable() {
    TemplateGridModal = $('#TemplateGridModal');
    TemplateDatatable = $('#modal_datatable_TemplateReports').KTDatatable({
        // datasource definition
        data: {
            type: 'remote',
            source: {
                read: {
                    method: 'GET',
                    url: 'Canvas?handler=AllPagedTemplates',
                    map: function (raw) {
                        // sample data mapping
                        var dataSet = raw;
                        if (typeof raw.data !== 'undefined') {
                            dataSet = raw.data;
                        }
                        return dataSet;
                    }
                }
            },
            pageSize: 10,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true
        },

        // layout definition
        layout: {
            scroll: true, // enable/disable datatable scroll both horizontal and vertical when needed.
            height: 500, // datatable's body's fixed height
            minHeight: 500,
            footer: false, // display/hide footer
        },

        // column sorting
        sortable: true,

        pagination: true,

        search: {
            input: TemplateGridModal.find('#generalSearch')
        },

        // columns definition
        columns: [
            {
                field: 'templateTypeName',
                title: 'Template Type'
            },
            {
                field: 'reportingDatetime',
                title: 'Generation Date'
            },
            {
                field: 'addressedToCodes',
                title: 'To'
            },
            {
                field: 'remarks',
                title: 'Remarks'
            },
            {
                field: 'subscriberCode',
                width: 80,
                title: 'Initiator'
            },
            {
                field: 'Actions',
                title: 'Actions',
                sortable: false,
                width: 70,
                overflow: 'visible',
                autoHide: false,
                template: function (row) {
                    return '\
						<div class="dropdown">\
							<a href="javascript:;" class="btn btn-sm btn-clean btn-icon btn-icon-md" data-toggle="dropdown">\
                                <i class="la la-ellipsis-h"></i>\
                            </a>\
						  	<div class="dropdown-menu dropdown-menu-right">\
                                <a href="javascript:;" onclick="viewTemplate('+ row.id + ')" class="dropdown-item"><i class="la la-edit"></i>View Report</a>\
						  	</div >\
						</div>\
                ';
                }
            }
        ]
    });
}

var NotesDatatable = function (data) {
    var datatable = 'Notes';
    var deleteRow = false;
    noteGridModal = $('#NotesModal');
    noteDatatable = $('#modal_datatable_Notes').KTDatatable({
        // datasource definition
        data: {
            type: 'local',
            source: data,
            pageSize: 10
        },

        // layout definition
        layout: {
            scroll: true, // enable/disable datatable scroll both horizontal and vertical when needed.
            height: 500, // datatable's body's fixed height
            minHeight: 500,
            footer: false // display/hide footer
        },

        // column sorting
        sortable: true,

        pagination: true,

        search: {
            input: noteGridModal.find('#generalSearch')
        },

        // columns definition
        columns: [
            {
                field: 'description',
                title: 'Notes Description'
            },
            {
                field: 'Actions',
                title: 'Actions',
                sortable: false,
                width: 70,
                overflow: 'visible',
                autoHide: false,
                template: function (row) {
                    return '\
                	<a href="javascript:;" onclick="deleteRecord(\''+ row.noteId + '\',\'' + deleteRow + '\',\'' + datatable + '\')" class="btn btn-sm btn-clean btn-icon btn-icon-md m-0" title="Delete">\
                		<i class="la la-trash"></i>\
                	</a>\
                ';
                }
            }
        ]
    });

    noteGridModal.find('#kt_form_approved').on('change', function () {
        noteDatatable.search($(this).val().toLowerCase(), 'isApproved');
    });

    noteGridModal.find('#kt_form_approved').selectpicker();

    // fix datatable layout after modal shown
    noteDatatable.hide();
    var alreadyReloaded = false;
    noteGridModal.on('shown.bs.modal', function () {
        if (!alreadyReloaded) {
            var modalContent = $(this).find('.modal-content');
            noteDatatable.spinnerCallback(true, modalContent);

            noteDatatable.reload();

            noteDatatable.on('kt-datatable--on-layout-updated', function () {
                noteDatatable.show();
                noteDatatable.spinnerCallback(false, modalContent);
                noteDatatable.redraw();
            });
            alreadyReloaded = true;
        }
    });
};

//#region AAAS DataTables

function incidentDatatable() {

    incidentGridmodal = $('#IncidentModal');
    incidentDt = $('#modal_datatable_incidents').KTDatatable({

        data: {
            type: 'remote',
            source: {
                read: {
                    method: 'GET',
                    url: 'Canvas?handler=AllPagedIncidents',
                    map: function (raw) {
                        // sample data mapping
                        var dataSet = raw;
                        if (typeof raw.data !== 'undefined') {
                            dataSet = raw.data;
                        }
                        return dataSet;
                    }
                }
            },
            pageSize: 10,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true
        },

        // layout definition
        layout: {
            scroll: true, // enable/disable datatable scroll both horizontal and vertical when needed.
            height: 500, // datatable's body's fixed height
            minHeight: 500,
            footer: false // display/hide footer
        },

        // column sorting
        sortable: true,

        pagination: true,

        search: {
            input: incidentGridmodal.find('#generalSearch')
        },

        // columns definition
        columns: [
            {
                field: 'userContactNumber',
                title: 'Contact Number',
                width: 290
            },
            {
                field: 'latitude',
                title: 'Position Information',
                template: function (row) {
                    return (row.latitude !== null ? '<span style="display:block;font-weight:500;">Latitude: <span style="font-weight:normal">' + row.latitude + '</span></span>' : '') +
                        (row.longitude !== null ? '<span style="display:block;font-weight:500;">Longitude: <span style="font-weight:normal">' + row.longitude + '</span></span>' : '');
                }
            },
            {
                field: 'incidentType',
                title: 'Incident Type',
            },
            {
                field: 'description',
                title: 'Description',
            },

            {
                field: 'createdBy',
                title: 'Initiator',
                width: 90
            }
        ]
    });
}

function sosDatatable() {

    sosGridmodal = $('#SOSModal');
    sosDatatable = $('#modal_datatable_sos').KTDatatable({

        data: {
            type: 'remote',
            source: {
                read: {
                    method: 'GET',
                    url: 'Canvas?handler=AllPagedSOS',
                    map: function (raw) {
                        // sample data mapping
                        var dataSet = raw;
                        if (typeof raw.data !== 'undefined') {
                            dataSet = raw.data;
                        }
                        return dataSet;
                    }
                }
            },
            pageSize: 10,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true
        },

        // layout definition
        layout: {
            scroll: true, // enable/disable datatable scroll both horizontal and vertical when needed.
            height: 500, // datatable's body's fixed height
            minHeight: 500,
            footer: false // display/hide footer
        },

        // column sorting
        sortable: true,

        pagination: true,

        search: {
            input: sosGridmodal.find('#generalSearch')
        },

        // columns definition
        columns: [
            {
                field: 'userContactNumber',
                title: 'Contact Number',
                width: 290
            },
            {
                field: 'latitude',
                title: 'Position Information',
                template: function (row) {
                    return (row.latitude !== null ? '<span style="display:block;font-weight:500;">Latitude: <span style="font-weight:normal">' + row.latitude + '</span></span>' : '') +
                        (row.longitude !== null ? '<span style="display:block;font-weight:500;">Longitude: <span style="font-weight:normal">' + row.longitude + '</span></span>' : '');
                }
            },
            {
                field: 'userIMEI',
                title: 'User IMEI',
            },
            {
                field: 'address',
                title: 'Address',
            },

            {
                field: 'createdBy',
                title: 'Initiator',
                width: 90
            }
        ]
    });
}

//#endregion AAAS DataTables

var stakeholdersDatatable = function (data) {
    var datatable = 'Stakeholder';
    var deleteRow = false;
    subsGridModal = $('#StakeholdersModal');
    subsDatatable = $('#modal_datatable_stakeholders').KTDatatable({
        // datasource definition
        data: {
            type: 'local',
            source: data,
            pageSize: 10
        },

        // layout definition
        layout: {
            scroll: true, // enable/disable datatable scroll both horizontal and vertical when needed.
            height: 500, // datatable's body's fixed height
            minHeight: 500,
            footer: false // display/hide footer
        },

        // column sorting
        sortable: true,

        pagination: true,

        search: {
            input: subsGridModal.find('#generalSearch')
        },

        // columns definition
        columns: [
            {
                field: 'subscriberName',
                title: 'Name'
            },
            {
                field: 'subscriberCode',
                title: 'Code'
            },
            {
                field: 'address',
                title: 'Address'
            },
            {
                field: 'stateCode',
                title: 'State Code'
            },
            {
                field: 'city',
                title: 'City'
            },
            {
                field: 'zip',
                title: 'Zip'
            },
            {
                field: 'email',
                title: 'Email'
            },
            {
                field: 'phone',
                title: 'Phone'
            },
            {
                field: 'subscriberTimeZone',
                title: 'Time Zone'
            },
            {
                field: 'isApproved',
                title: 'Approved',
                autoHide: false,
                //callback function support for column rendering
                template: function (row) {
                    var approved = {
                        'true': { 'title': 'True', 'class': 'kt-badge--success' },
                        'false': { 'title': 'False', 'class': 'kt-badge--danger' }
                    };
                    return '<span class="kt-badge ' + approved[row.isApproved].class + ' kt-badge--inline kt-badge--pill">' + approved[row.isApproved].title + '</span>';
                }
            },
            {
                field: 'Actions',
                title: 'Actions',
                sortable: false,
                width: 70,
                overflow: 'visible',
                autoHide: false,
                template: function (row) {
                    return '\
                	<a href="javascript:;" onclick="editStakeholder('+ row.subscriberId + ')" class="btn btn-sm btn-clean btn-icon btn-icon-md m-0" title="Edit details">\
                		<i class="la la-edit"></i>\
                	</a>\
                	<a href="javascript:;" onclick="deleteRecord(\''+ row.subscriberId + '\',\'' + deleteRow + '\', \'' + datatable + '\')" class="btn btn-sm btn-clean btn-icon btn-icon-md m-0" title="Delete">\
                		<i class="la la-trash"></i>\
                	</a>\
                ';
                }
            }
        ]
    });


    subsGridModal.find('#kt_form_approved').on('change', function () {
        subsDatatable.search($(this).val().toLowerCase(), 'isApproved');
    });

    subsGridModal.find('#kt_form_approved').selectpicker();

    // fix datatable layout after modal shown
    subsDatatable.hide();
    var alreadyReloaded = false;
    subsGridModal.on('shown.bs.modal', function () {
        if (!alreadyReloaded) {
            var modalContent = $(this).find('.modal-content');
            subsDatatable.spinnerCallback(true, modalContent);

            subsDatatable.reload();

            subsDatatable.on('kt-datatable--on-layout-updated', function () {
                subsDatatable.show();
                subsDatatable.spinnerCallback(false, modalContent);
                subsDatatable.redraw();
            });
            alreadyReloaded = true;
        }
    });
};

var usersDatatable = function (data) {
    var datatable = 'User';
    var deleteRow = false;
    usrsGridModal = $('#UsersModal');
    usrsDatatable = $('#modal_datatable_users').KTDatatable({
        // datasource definition
        data: {
            type: 'local',
            source: data,
            pageSize: 10
        },

        // layout definition
        layout: {
            scroll: true, // enable/disable datatable scroll both horizontal and vertical when needed.
            height: 500, // datatable's body's fixed height
            minHeight: 500,
            footer: false // display/hide footer
        },

        // column sorting
        sortable: true,

        pagination: true,

        search: {
            input: usrsGridModal.find('#generalSearch')
        },

        // columns definition
        columns: [
            {
                field: 'userName',
                title: 'User Name'
            },
            {
                field: 'email',
                title: 'Email'
            },
            {
                field: 'subscriberName',
                title: 'Subscriber Name'
            },
            {
                field: 'Actions',
                title: 'Actions',
                sortable: false,
                width: 70,
                overflow: 'visible',
                autoHide: false,
                template: function (row) {
                    return "<a href='javascript:;' class='btn btn-sm btn-clean btn-icon btn-icon-md m-0' title='Delete' onclick=\"deleteRecord(" + row.id + ",'" + deleteRow + "','" + datatable + "')\"><i class='la la-trash'></i></a>";
                }
            }
        ]
    });


    usrsGridModal.find('#kt_form_approved').on('change', function () {
        usrsDatatable.search($(this).val().toLowerCase(), 'isApproved');
    });

    usrsGridModal.find('#kt_form_approved').selectpicker();

    // fix datatable layout after modal shown
    usrsDatatable.hide();
    var alreadyReloaded = false;
    usrsGridModal.on('shown.bs.modal', function () {
        if (!alreadyReloaded) {
            var modalContent = $(this).find('.modal-content');
            usrsDatatable.spinnerCallback(true, modalContent);

            usrsDatatable.reload();

            usrsDatatable.on('kt-datatable--on-layout-updated', function () {
                usrsDatatable.show();
                usrsDatatable.spinnerCallback(false, modalContent);
                usrsDatatable.redraw();
            });

            alreadyReloaded = true;
        }
    });
};

var userTypesDatatable = function (data) {
    var datatable = 'UserType';
    var deleteRow = false;
    usrTypeGridModal = $('#UserTypesModal');
    usrTypeDatatable = $('#modal_datatable_userTypes').KTDatatable({
        // datasource definition
        data: {
            type: 'local',
            source: data,
            pageSize: 10
        },

        // layout definition
        layout: {
            scroll: true, // enable/disable datatable scroll both horizontal and vertical when needed.
            height: 500, // datatable's body's fixed height
            minHeight: 500,
            footer: false // display/hide footer
        },

        // column sorting
        sortable: true,

        pagination: true,

        search: {
            input: usrTypeGridModal.find('#generalSearch')
        },

        // columns definition
        columns: [
            {
                field: 'userTypeName',
                title: 'User Type'
            },
            {
                field: 'Actions',
                title: 'Actions',
                sortable: false,
                width: 70,
                overflow: 'visible',
                autoHide: false,
                template: function (row) {
                    return '\
                	<a href="javascript:;" onclick="deleteRecord(\''+ row.userTypeId + '\',\'' + deleteRow + '\',\'' + datatable + '\')" class="btn btn-sm btn-clean btn-icon btn-icon-md m-0" title="Delete">\
                		<i class="la la-trash"></i>\
                	</a>\
                ';
                }
            }
        ]
    });

    //<a href="javascript:;" onclick="editUserType('+ row.userTypeId + ')" class="btn btn-sm btn-clean btn-icon btn-icon-md m-0" title="Edit details">\
    //                		<i class="la la-edit"></i>\
    //                	</a>\

    usrTypeGridModal.find('#kt_form_approved').on('change', function () {
        usrTypeDatatable.search($(this).val().toLowerCase(), 'isApproved');
    });

    usrTypeGridModal.find('#kt_form_approved').selectpicker();

    // fix datatable layout after modal shown
    usrTypeDatatable.hide();
    var alreadyReloaded = false;
    usrTypeGridModal.on('shown.bs.modal', function () {
        if (!alreadyReloaded) {
            var modalContent = $(this).find('.modal-content');
            usrTypeDatatable.spinnerCallback(true, modalContent);

            usrTypeDatatable.reload();

            usrTypeDatatable.on('kt-datatable--on-layout-updated', function () {
                usrTypeDatatable.show();
                usrTypeDatatable.spinnerCallback(false, modalContent);
                usrTypeDatatable.redraw();
            });

            alreadyReloaded = true;
        }
    });
};

var newsFeedTypesDatatable = function (data) {
    var datatable = 'NewsFeedType';
    var deleteRow = false;
    nwsFeedTypeModal = $('#NewsFeedTypesModal');
    nwsFeedTypeDatatable = $('#modal_datatable_newsFeedTypes').KTDatatable({
        // datasource definition
        data: {
            type: 'local',
            source: data,
            pageSize: 10
        },

        // layout definition
        layout: {
            scroll: true, // enable/disable datatable scroll both horizontal and vertical when needed.
            height: 500, // datatable's body's fixed height
            minHeight: 500,
            footer: false // display/hide footer
        },

        // column sorting
        sortable: true,

        pagination: true,

        search: {
            input: nwsFeedTypeModal.find('#generalSearch')
        },

        // columns definition
        columns: [
            {
                field: 'newsFeedTypeName',
                title: 'News Feed Type'
            },
            {
                field: 'Actions',
                title: 'Actions',
                sortable: false,
                width: 70,
                overflow: 'visible',
                autoHide: false,
                template: function (row) {
                    return '\
                	<a href="javascript:;" onclick="editNewsFeedType('+ row.newsFeedTypeId + ')" class="btn btn-sm btn-clean btn-icon btn-icon-md m-0" title="Edit details">\
                		<i class="la la-edit"></i>\
                	</a>\
                	<a href="javascript:;" onclick="deleteRecord(\''+ row.newsFeedTypeId + '\',\'' + deleteRow + '\',\'' + datatable + '\')" class="btn btn-sm btn-clean btn-icon btn-icon-md m-0" title="Delete">\
                		<i class="la la-trash"></i>\
                	</a>\
                ';
                }
            }
        ]
    });


    nwsFeedTypeModal.find('#kt_form_approved').on('change', function () {
        nwsFeedTypeDatatable.search($(this).val().toLowerCase(), 'isApproved');
    });

    nwsFeedTypeModal.find('#kt_form_approved').selectpicker();

    // fix datatable layout after modal shown
    nwsFeedTypeDatatable.hide();
    var alreadyReloaded = false;
    nwsFeedTypeModal.on('shown.bs.modal', function () {
        if (!alreadyReloaded) {
            var modalContent = $(this).find('.modal-content');
            nwsFeedTypeDatatable.spinnerCallback(true, modalContent);

            nwsFeedTypeDatatable.reload();

            nwsFeedTypeDatatable.on('kt-datatable--on-layout-updated', function () {
                nwsFeedTypeDatatable.show();
                nwsFeedTypeDatatable.spinnerCallback(false, modalContent);
                nwsFeedTypeDatatable.redraw();
            });

            alreadyReloaded = true;
        }
    });
};

var coiStatusesDatatable = function (data) {
    var datatable = 'COIStatus';
    var deleteRow = false;
    coiStGridModal = $('#COIStatusesModal');
    coiStDatatable = $('#modal_datatable_coiStatuses').KTDatatable({
        // datasource definition
        data: {
            type: 'local',
            source: data,
            pageSize: 10
        },

        // layout definition
        layout: {
            scroll: true, // enable/disable datatable scroll both horizontal and vertical when needed.
            height: 500, // datatable's body's fixed height
            minHeight: 500,
            footer: false // display/hide footer
        },

        // column sorting
        sortable: true,

        pagination: true,

        search: {
            input: coiStGridModal.find('#generalSearch')
        },

        // columns definition
        columns: [
            {
                field: 'coIstatus',
                title: 'COI Status'
            },
            {
                field: 'Actions',
                title: 'Actions',
                sortable: false,
                width: 70,
                overflow: 'visible',
                autoHide: false,
                template: function (row) {
                    return '\
                	<a href="javascript:;" onclick="editCOIStatus('+ row.coiStatusId + ')" class="btn btn-sm btn-clean btn-icon btn-icon-md m-0" title="Edit details">\
                		<i class="la la-edit"></i>\
                	</a>\
                	<a href="javascript:;" onclick="deleteRecord(\''+ row.coiStatusId + '\',\'' + deleteRow + '\',\'' + datatable + '\')" class="btn btn-sm btn-clean btn-icon btn-icon-md m-0" title="Delete">\
                		<i class="la la-trash"></i>\
                	</a>\
                ';
                }
            }
        ]
    });


    coiStGridModal.find('#kt_form_approved').on('change', function () {
        coiStDatatable.search($(this).val().toLowerCase(), 'isApproved');
    });

    coiStGridModal.find('#kt_form_approved').selectpicker();

    // fix datatable layout after modal shown
    coiStDatatable.hide();
    var alreadyReloaded = false;
    coiStGridModal.on('shown.bs.modal', function () {
        if (!alreadyReloaded) {
            var modalContent = $(this).find('.modal-content');
            coiStDatatable.spinnerCallback(true, modalContent);

            coiStDatatable.reload();

            coiStDatatable.on('kt-datatable--on-layout-updated', function () {
                coiStDatatable.show();
                coiStDatatable.spinnerCallback(false, modalContent);
                coiStDatatable.redraw();
            });

            alreadyReloaded = true;
        }
    });
};

var coiTypesDatatable = function (data) {
    var datatable = 'COIType';
    var deleteRow = false;
    coiTypeGridModal = $('#COITypesModal');
    coiTypeDatatable = $('#modal_datatable_coiTypes').KTDatatable({
        // datasource definition
        data: {
            type: 'local',
            source: data,
            pageSize: 10
        },

        // layout definition
        layout: {
            scroll: true, // enable/disable datatable scroll both horizontal and vertical when needed.
            height: 500, // datatable's body's fixed height
            minHeight: 500,
            footer: false // display/hide footer
        },

        // column sorting
        sortable: true,

        pagination: true,

        search: {
            input: coiTypeGridModal.find('#generalSearch')
        },

        // columns definition
        columns: [
            {
                field: 'coiTypeName',
                title: 'COI Type'
            },
            {
                field: 'Actions',
                title: 'Actions',
                sortable: false,
                width: 70,
                overflow: 'visible',
                autoHide: false,
                template: function (row) {
                    return '\
                	<a href="javascript:;" onclick="editCOIType('+ row.coiTypeId + ')" class="btn btn-sm btn-clean btn-icon btn-icon-md m-0" title="Edit details">\
                		<i class="la la-edit"></i>\
                	</a>\
                	<a href="javascript:;" onclick="deleteRecord(\''+ row.coiTypeId + '\',\'' + deleteRow + '\',\'' + datatable + '\')" class="btn btn-sm btn-clean btn-icon btn-icon-md m-0" title="Delete">\
                		<i class="la la-trash"></i>\
                	</a>\
                ';
                }
            }
        ]
    });


    coiTypeGridModal.find('#kt_form_approved').on('change', function () {
        coiTypeDatatable.search($(this).val().toLowerCase(), 'isApproved');
    });

    coiTypeGridModal.find('#kt_form_approved').selectpicker();

    // fix datatable layout after modal shown
    coiTypeDatatable.hide();
    var alreadyReloaded = false;
    coiTypeGridModal.on('shown.bs.modal', function () {
        if (!alreadyReloaded) {
            var modalContent = $(this).find('.modal-content');
            coiTypeDatatable.spinnerCallback(true, modalContent);

            coiTypeDatatable.reload();

            coiTypeDatatable.on('kt-datatable--on-layout-updated', function () {
                coiTypeDatatable.show();
                coiTypeDatatable.spinnerCallback(false, modalContent);
                coiTypeDatatable.redraw();
            });

            alreadyReloaded = true;
        }
    });
};

var templateTypesDatatable = function (data) {
    var datatable = 'TemplateType';
    var deleteRow = false;
    templateTypeGridModal = $('#TemplateTypesModal');
    templateTypeDatatable = $('#modal_datatable_TemplateTypes').KTDatatable({
        // datasource definition
        data: {
            type: 'local',
            source: data,
            pageSize: 10
        },

        // layout definition
        layout: {
            scroll: true, // enable/disable datatable scroll both horizontal and vertical when needed.
            height: 500, // datatable's body's fixed height
            minHeight: 500,
            footer: false // display/hide footer
        },

        // column sorting
        sortable: true,

        pagination: true,

        search: {
            input: templateTypeGridModal.find('#generalSearch')
        },

        // columns definition
        columns: [
            {
                field: 'templateTypeName',
                title: 'Template Type'
            },
            {
                field: 'Actions',
                title: 'Actions',
                sortable: false,
                width: 70,
                overflow: 'visible',
                autoHide: false,
                template: function (row) {
                    return '\
                	<a href="javascript:;" onclick="editTemplateType('+ row.templateTypeId + ')" class="btn btn-sm btn-clean btn-icon btn-icon-md m-0" title="Edit details">\
                		<i class="la la-edit"></i>\
                	</a>\
                	<a href="javascript:;" onclick="deleteRecord(\''+ row.templateTypeId + '\',\'' + deleteRow + '\',\'' + datatable + '\')" class="btn btn-sm btn-clean btn-icon btn-icon-md m-0" title="Delete">\
                		<i class="la la-trash"></i>\
                	</a>\
                ';
                }
            }
        ]
    });

    templateTypeGridModal.find('#kt_form_approved').on('change', function () {
        templateTypeDatatable.search($(this).val().toLowerCase(), 'isApproved');
    });

    templateTypeGridModal.find('#kt_form_approved').selectpicker();

    // fix datatable layout after modal shown
    templateTypeDatatable.hide();
    var alreadyReloaded = false;
    templateTypeGridModal.on('shown.bs.modal', function () {
        if (!alreadyReloaded) {
            var modalContent = $(this).find('.modal-content');
            templateTypeDatatable.spinnerCallback(true, modalContent);

            templateTypeDatatable.reload();

            templateTypeDatatable.on('kt-datatable--on-layout-updated', function () {
                templateTypeDatatable.show();
                templateTypeDatatable.spinnerCallback(false, modalContent);
                templateTypeDatatable.redraw();
            });

            alreadyReloaded = true;
        }
    });
};

var threatLevelsDatatable = function (data) {
    var datatable = 'ThreatLevel';
    var deleteRow = false;
    thrLevelGridModal = $('#threatLevelsModal');
    thrLevelDatatable = $('#modal_datatable_threatLevels').KTDatatable({
        // datasource definition
        data: {
            type: 'local',
            source: data,
            pageSize: 10
        },

        // layout definition
        layout: {
            scroll: true, // enable/disable datatable scroll both horizontal and vertical when needed.
            height: 500, // datatable's body's fixed height
            minHeight: 500,
            footer: false // display/hide footer
        },

        // column sorting
        sortable: true,

        pagination: true,

        search: {
            input: thrLevelGridModal.find('#generalSearch')
        },

        // columns definition
        columns: [
            {
                field: 'threatName',
                title: 'Threat Name'
            },
            {
                field: 'Actions',
                title: 'Actions',
                sortable: false,
                width: 70,
                overflow: 'visible',
                autoHide: false,
                template: function (row) {
                    return '\
                	<a href="javascript:;" onclick="editThreatLevel('+ row.threatId + ')" class="btn btn-sm btn-clean btn-icon btn-icon-md m-0" title="Edit details">\
                		<i class="la la-edit"></i>\
                	</a>\
                	<a href="javascript:;" onclick="deleteRecord(\''+ row.threatId + '\',\'' + deleteRow + '\',\'' + datatable + '\')" class="btn btn-sm btn-clean btn-icon btn-icon-md m-0" title="Delete">\
                		<i class="la la-trash"></i>\
                	</a>\
                ';
                }
            }
        ]
    });


    thrLevelGridModal.find('#kt_form_approved').on('change', function () {
        thrLevelDatatable.search($(this).val().toLowerCase(), 'isApproved');
    });

    thrLevelGridModal.find('#kt_form_approved').selectpicker();

    // fix datatable layout after modal shown
    thrLevelDatatable.hide();
    var alreadyReloaded = false;
    thrLevelGridModal.on('shown.bs.modal', function () {
        if (!alreadyReloaded) {
            var modalContent = $(this).find('.modal-content');
            thrLevelDatatable.spinnerCallback(true, modalContent);

            thrLevelDatatable.reload();

            thrLevelDatatable.on('kt-datatable--on-layout-updated', function () {
                thrLevelDatatable.show();
                thrLevelDatatable.spinnerCallback(false, modalContent);
                thrLevelDatatable.redraw();
            });

            alreadyReloaded = true;
        }
    });
};

var infoConLevelsDatatable = function (data) {
    var datatable = 'InfoConLevel';
    var deleteRow = false;
    infoConGridModal = $('#InfoConLevelsModal');
    infoConDatatable = $('#modal_datatable_infoConLevels').KTDatatable({
        // datasource definition
        data: {
            type: 'local',
            source: data,
            pageSize: 10
        },

        // layout definition
        layout: {
            scroll: true, // enable/disable datatable scroll both horizontal and vertical when needed.
            height: 500, // datatable's body's fixed height
            minHeight: 500,
            footer: false // display/hide footer
        },

        // column sorting
        sortable: true,

        pagination: true,

        search: {
            input: infoConGridModal.find('#generalSearch')
        },

        // columns definition
        columns: [
            {
                field: 'infoConfidenceLevelName',
                title: 'Confidence Level'
            },
            {
                field: 'Actions',
                title: 'Actions',
                sortable: false,
                width: 70,
                overflow: 'visible',
                autoHide: false,
                template: function (row) {
                    return '\
                	<a href="javascript:;" onclick="editInfoConLevel('+ row.infoConfidenceLevelId + ')" class="btn btn-sm btn-clean btn-icon btn-icon-md m-0" title="Edit details">\
                		<i class="la la-edit"></i>\
                	</a>\
                	<a href="javascript:;" onclick="deleteRecord(\''+ row.infoConfidenceLevelId + '\',\'' + deleteRow + '\',\'' + datatable + '\')" class="btn btn-sm btn-clean btn-icon btn-icon-md m-0" title="Delete">\
                		<i class="la la-trash"></i>\
                	</a>\
                ';
                }
            }
        ]
    });

    infoConGridModal.find('#kt_form_approved').on('change', function () {
        infoConDatatable.search($(this).val().toLowerCase(), 'isApproved');
    });

    infoConGridModal.find('#kt_form_approved').selectpicker();

    // fix datatable layout after modal shown
    infoConDatatable.hide();
    var alreadyReloaded = false;
    infoConGridModal.on('shown.bs.modal', function () {
        if (!alreadyReloaded) {
            var modalContent = $(this).find('.modal-content');
            infoConDatatable.spinnerCallback(true, modalContent);

            infoConDatatable.reload();

            infoConDatatable.on('kt-datatable--on-layout-updated', function () {
                infoConDatatable.show();
                infoConDatatable.spinnerCallback(false, modalContent);
                infoConDatatable.redraw();
            });

            alreadyReloaded = true;
        }
    });
};

tracklistDt = $('#trackList').DataTable({ "ordering": false });

//#endregion Datatables

//#region Modals

function addStakeholder() {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=Stakeholder').done(function (data) {
        placeholderElement.html(data);
        modalDraggable();
        $('#CRUDStakeholderModal').modal('show');
    });
}
function addUser() {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=User').done(function (data) {
        placeholderElement.html(data);
        bindDropdowns();
        modalDraggable();
        $('#CRUDUserModal').modal('show');
    });
}
function addUserType() {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=UserType').done(function (data) {
        placeholderElement.html(data);
        modalDraggable();
        $('#UserTypeModal').modal('show');
    });
}
function addNewsFeedType() {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=NewsFeedType').done(function (data) {
        placeholderElement.html(data);
        modalDraggable();
        $('#NewsFeedTypeModal').modal('show');
    });
}
function addCOIStatus() {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=COIStatus').done(function (data) {
        placeholderElement.html(data);
        modalDraggable();
        $('#COIStatusModal').modal('show');
    });
}
function addCOIType() {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=COIType').done(function (data) {
        placeholderElement.html(data);
        modalDraggable();
        $('#CRUDCOITypeModal').modal('show');
    });
}
function addTemplateType() {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=TemplateType').done(function (data) {
        placeholderElement.html(data);
        modalDraggable();
        $('#CRUDTemplateTypeModal').modal('show');
    });
}
function addThreatLevel() {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=NatureOfThreat').done(function (data) {
        placeholderElement.html(data);
        modalDraggable();
        $('#CRUDNOTModal').modal('show');
    });
}
function addInfoConLevel() {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=InfoConLevel').done(function (data) {
        placeholderElement.html(data);
        modalDraggable();
        $('#InfoConLevelModal').modal('show');
    });
}
function addTemplate() {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=Template').done(function (data) {
        placeholderElement.html(data);
        modalDraggable();
        bindDropdowns();
        $('#TemplateModal').modal('show');
    });
}
function addNotes() {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=Notes').done(function (data) {
        placeholderElement.html(data);
        modalDraggable();
        $('#CRUDNotesModal').modal('show');
    });
}

function editStakeholder(subscriberId) {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=Stakeholder&subsId=' + subscriberId).done(function (data) {
        placeholderElement.html(data);
        modalDraggable();
        $('#EditStakeholderModal').modal('show');
    });
}
function editUserType(userTypeId) {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=UserType&userTypeId=' + userTypeId).done(function (data) {
        placeholderElement.html(data);
        modalDraggable();
        placeholderElement.find('#EditUserTypeModal').modal('show');
    });
}
function editNewsFeedType(newsFeedTypeId) {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=NewsFeedType&newsFeedTypeId=' + newsFeedTypeId).done(function (data) {
        placeholderElement.html(data);
        modalDraggable();
        placeholderElement.find('#EditNewsFeedTypeModal').modal('show');
    });
}
function editCOIStatus(coiStatusId) {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=COIStatus&coiStatusId=' + coiStatusId).done(function (data) {
        placeholderElement.html(data);
        modalDraggable();
        placeholderElement.find('#EditCOIStatusModal').modal('show');
    });
}
function editCOIType(coiTypeId) {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=COIType&coiTypeId=' + coiTypeId).done(function (data) {
        placeholderElement.html(data);
        modalDraggable();
        placeholderElement.find('#EditCOITypeModal').modal('show');
    });
}
function editTemplateType(templateTypeId) {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=TemplateType&templateTypeId=' + templateTypeId).done(function (data) {
        placeholderElement.html(data);
        modalDraggable();
        placeholderElement.find('#EditTemplateTypeModal').modal('show');
    });
}
function editThreatLevel(threatId) {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=NatureOfThreat&threatId=' + threatId).done(function (data) {
        placeholderElement.html(data);
        modalDraggable();
        placeholderElement.find('#EditNOTModal').modal('show');
    });
}
function editInfoConLevel(infoConLevelId) {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=InfoConLevel&infoConLevelId=' + infoConLevelId).done(function (data) {
        placeholderElement.html(data);
        modalDraggable();
        placeholderElement.find('#EditInfoConLevelModal').modal('show');
    });
}

function deleteRecord(Id, deleteRow, datatableName) {
    if (deleteRow === 'false') {
        $('#sweetAlert').find('.modal-body #Id').val(Id);
        $('#sweetAlert').find('.modal-body #datatableName').val(datatableName);
        $('#sweetAlert').modal('show');
    }
    else {
        if (datatableName === 'Stakeholder') {
            fetch('Canvas?handler=DeleteStakeholder&subsId=' + Id, {
                method: 'GET'
            }).then(function (response) {
                if (response.status === 200) {
                    $('#sweetAlert').modal('hide');
                }
            }).catch((err) => console.log(err));
            return false;
        }
        if (datatableName === 'User') {
            fetch('Canvas?handler=DeleteUser&Id=' + Id, {
                method: 'GET'
            }).then(function (response) {
                if (response.status === 200) {
                    $('#sweetAlert').modal('hide');
                }
            }).catch((err) => console.log(err));
            return false;
        }
        if (datatableName === 'UserType') {
            fetch('Canvas?handler=DeleteUserType&userTypeId=' + Id, {
                method: 'GET'
            }).then(function (response) {
                if (response.status === 200) {
                    $('#sweetAlert').modal('hide');
                }
            }).catch((err) => console.log(err));
            return false;
        }
        if (datatableName === 'NewsFeedType') {
            fetch('Canvas?handler=DeleteNewsFeedType&newsFeedTypeId=' + Id, {
                method: 'GET'
            }).then(function (response) {
                if (response.status === 200) {
                    $('#sweetAlert').modal('hide');
                }
            }).catch((err) => console.log(err));
            return false;
        }
        if (datatableName === 'COIStatus') {
            fetch('Canvas?handler=DeleteCOIStatus&coiStatusId=' + Id, {
                method: 'GET'
            }).then(function (response) {
                if (response.status === 200) {
                    $('#sweetAlert').modal('hide');
                }
            }).catch((err) => console.log(err));
            return false;
        }
        if (datatableName === 'COIType') {
            fetch('Canvas?handler=DeleteCOIType&coiTypeId=' + Id, {
                method: 'GET'
            }).then(function (response) {
                if (response.status === 200) {
                    $('#sweetAlert').modal('hide');
                }
            }).catch((err) => console.log(err));
            return false;
        }
        if (datatableName === 'TemplateType') {
            fetch('Canvas?handler=DeleteTemplateType&templateTypeId=' + Id, {
                method: 'GET'
            }).then(function (response) {
                if (response.status === 200) {
                    $('#sweetAlert').modal('hide');
                }
            }).catch((err) => console.log(err));
            return false;
        }
        if (datatableName === 'Notes') {
            fetch('Canvas?handler=DeleteNotes&notesId=' + Id, {
                method: 'GET'
            }).then(function (response) {
                if (response.status === 200) {
                    $('#sweetAlert').modal('hide');
                }
            }).catch((err) => console.log(err));
            return false;
        }
        if (datatableName === 'ThreatLevel') {
            fetch('Canvas?handler=DeleteNatureOfThreat&threatId=' + Id, {
                method: 'GET'
            }).then(function (response) {
                if (response.status === 200) {
                    $('#sweetAlert').modal('hide');
                }
            }).catch((err) => console.log(err));
            return false;
        }
        if (datatableName === 'InfoConLevel') {
            fetch('Canvas?handler=DeleteInfoConLevel&infoConLevelId=' + Id, {
                method: 'GET'
            }).then(function (response) {
                if (response.status === 200) {
                    $('#sweetAlert').modal('hide');
                }
            }).catch((err) => console.log(err));
            return false;
        }
        if (datatableName === 'DR') {
            fetch('Canvas?handler=DeleteDR&DRId=' + Id, {
                method: 'GET'
            }).then(function (response) {
                if (response.status === 200) {
                    $('.modal').modal('hide');
                }
            }).catch((err) => console.log(err));
            return false;
        }
    }
}


//#region AAAS

function openIncidentGrid() {
    modalDraggable();
    incidentDatatable();
    $('#IncidentModal').modal('show');
}
function openSOSGrid() {
    modalDraggable();
    sosDatatable();
    $('#SOSModal').modal('show');
}

//#endregion AAAS


function openStakeholderGrid() {
    if (typeof subsDatatable === 'undefined') {
        $.ajax({
            url: 'Canvas?handler=AllStakeholders',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                stakeholdersDatatable(response);
            }
        });
    }
    modalDraggable();
    $('#StakeholdersModal').modal('show');
}
function openUserGrid() {
    if (typeof usrsDatatable === 'undefined') {
        $.ajax({
            url: 'Canvas?handler=AllUsers',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                usersDatatable(response);
            }
        });
    }
    modalDraggable();
    $('#UsersModal').modal('show');
}
function openUserTypeGrid() {
    if (typeof usrTypeDatatable === 'undefined') {
        $.ajax({
            url: 'Canvas?handler=AllUserTypes',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                userTypesDatatable(response);
            }
        });
    }
    modalDraggable();
    $('#UserTypesModal').modal('show');
}
function openNewsFeedTypeGrid() {
    if (typeof nwsFeedTypeDatatable === 'undefined') {
        $.ajax({
            url: 'Canvas?handler=AllNewsFeedTypes',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                newsFeedTypesDatatable(response);
            }
        });
    }
    modalDraggable();
    $('#NewsFeedTypesModal').modal('show');
}
function openCOIStatusGrid() {
    if (typeof coiStDatatable === 'undefined') {
        $.ajax({
            url: 'Canvas?handler=AllCOIStatuses',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                coiStatusesDatatable(response);
            }
        });
    }
    modalDraggable();
    $('#COIStatusesModal').modal('show');
}
function openCOITypeGrid() {
    if (typeof coiTypeDatatable === 'undefined') {
        $.ajax({
            url: 'Canvas?handler=COITypes',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                coiTypesDatatable(response);
            }
        });
    }
    modalDraggable();
    $('#COITypesModal').modal('show');
}
function openTemplateTypeGrid() {
    if (typeof templateTypeDatatable === 'undefined') {
        $.ajax({
            url: 'Canvas?handler=TemplateTypes',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                console.log(response);
                templateTypesDatatable(response);
            }
        });
    }
    modalDraggable();
    $('#TemplateTypesModal').modal('show');
}
function openThreatGrid() {
    if (typeof thrLevelDatatable === 'undefined') {
        $.ajax({
            url: 'Canvas?handler=AllNatureOfThreats',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                threatLevelsDatatable(response);
            }
        });
    }
    modalDraggable();
    $('#threatLevelsModal').modal('show');
}
function openInfoConLevelGrid() {
    if (typeof infoConDatatable === 'undefined') {
        $.ajax({
            url: 'Canvas?handler=AllInfoConLevels',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                infoConLevelsDatatable(response);
            }
        });
    }
    modalDraggable();
    $('#InfoConLevelsModal').modal('show');
}

function openPRGrid() {
    modalDraggable();
    prReportsDatatable();
    $('#PRGridModal').modal('show');
}
function openSRGrid() {
    modalDraggable();
    subsReportsDatatable();
    $('#SRGridModal').modal('show');
}
function openCOIGrid() {
    modalDraggable();
    coiReportsDatatable();
    $('#COIGridModal').modal('show');
}
function openARGrid() {
    modalDraggable();
    ampReportsDatatable();
    $('#ARGridModal').modal('show');
}
function openLostContactGrid() {
    modalDraggable();
    lostReportsDatatable();
    $('#LostContactGridModal').modal('show');
}
function openDropContactGrid() {
    modalDraggable();
    dropReportsDatatable();
    $('#DropContactGridModal').modal('show');
}
function openAARGrid() {
    modalDraggable();
    AAReportsDatatable();
    $('#AARGridModal').modal('show');
}
function openTemplateGrid() {
    modalDraggable();
    TemplateReportsDatatable();
    $('#TemplateGridModal').modal('show');
}
function openNotesGrid() {
    if (typeof noteDatatable === 'undefined') {
        $.ajax({
            url: 'Canvas?handler=AllNotes',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                console.log(response);
                NotesDatatable(response);
            }
        });
    }
    modalDraggable();
    $('#NotesModal').modal('show');
}

function createAAR(drId, initiator, aarCreated) {
    if (initiator === $('#loggedInSubsCode').val()) {
        if (aarCreated === false || aarCreated === 'false' || aarCreated === null || aarCreated === 'null') {
            var placeholderElement = $('#modal-placeholder');
            $.get('Canvas?handler=AAR&DRId=' + drId).done(function (data) {
                placeholderElement.html(data);
                modalDraggable();
                bindDropdowns();
                placeholderElement.find('#AfterActionReportModal').modal('show');
            });
        }
        else {
            $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('After Action Report is already created against this Drop Report.');
            $('#sweetAlertGridMessages').modal('show');
        }
    }
    else {
        $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('Only ' + initiator + ' can create Lost Report.');
        $('#sweetAlertGridMessages').modal('show');
    }
}
function showLRReportPR(prId, mmsi, course, speed, coiNumber, isLost, isDropped) {
    if ($('#loggedInSubsId').val() === "1") {
        if (isDropped === false || isDropped === 'false' || isDropped === null || isDropped === 'null') {
            if (isLost === false || isLost === 'false' || isLost === null || isLost === 'null') {
                if (coiNumber === 'null' || coiNumber === null) {
                    var placeholderElement = $('#modal-placeholder');
                    $.get('Canvas?handler=LR&PRId=' + prId).done(function (data) {
                        placeholderElement.html(data);
                        modalDraggable();
                        bindDropdowns();
                        if (mmsi === 'null' && course === 'null' && speed === 'null') {
                            $('.lostReportTrackFields').hide();
                        }
                        placeholderElement.find('#LostReportModal').modal('show');
                    });
                }
                else {
                    $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('COI is already generated against this Preliminary Report.');
                    $('#sweetAlertGridMessages').modal('show');
                }
            }
            else {
                $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('This Preliminary Report is already lost.');
                $('#sweetAlertGridMessages').modal('show');
            }
        }
        else {
            $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('This Preliminary Report is dropped. So, you cannot create the Lost Report for this.');
            $('#sweetAlertGridMessages').modal('show');
        }
    }
    else {
        $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('Only JMICC can create Lost Report.');
        $('#sweetAlertGridMessages').modal('show');
    }
}
function showDRReportPR(prId, coiNumber, isDropped) {
    if ($('#loggedInSubsId').val() === "1") {
        if (isDropped === false || isDropped === 'false' || isDropped === null || isDropped === 'null') {
            if (coiNumber === 'null' || coiNumber === null) {
                var placeholderElement = $('#modal-placeholder');
                $.get('Canvas?handler=DR&prId=' + prId).done(function (data) {
                    placeholderElement.html(data);
                    modalDraggable();
                    bindDropdowns();
                    placeholderElement.find('#DropReportModal').modal('show');
                });
            }
            else {
                $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('COI is generated against this Preliminary Report. So, please drop it from COI Table.');
                $('#sweetAlertGridMessages').modal('show');
            }
        }
        else {
            $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('This Preliminary Report is already dropped.');
            $('#sweetAlertGridMessages').modal('show');
        }
    }
    else {
        $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('Only JMICC can create Drop Report.');
        $('#sweetAlertGridMessages').modal('show');
    }
}
function showLRReportCOI(coiId, mmsi, course, speed, isLost, isDropped) {
    if ($('#loggedInSubsId').val() === "1") {
        if (isDropped === false || isDropped === 'false' || isDropped === null || isDropped === 'null') {
            if (isLost === false || isLost === 'false' || isLost === null || isLost === 'null') {
                var placeholderElement = $('#modal-placeholder');
                $.get('Canvas?handler=LR&COIId=' + coiId).done(function (data) {
                    placeholderElement.html(data);
                    modalDraggable();
                    bindDropdowns();
                    if (mmsi === 'null' && course === 'null' && speed === 'null') {
                        $('.lostReportTrackFields').hide();
                    }
                    placeholderElement.find('#LostReportModal').modal('show');
                });
            }
            else {
                $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('This COI is already lost.');
                $('#sweetAlertGridMessages').modal('show');
            }
        }
        else {
            $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('This COI is dropped. So, you cannot create the Lost Report for this.');
            $('#sweetAlertGridMessages').modal('show');
        }
    }
    else {
        $('#sweetAlertGridMessages').find('Only JMICC can create Lost Report.');
        $('#sweetAlertGridMessages').modal('show');
    }
}
function showDRReportCOI(coiId, isDropped) {
    if ($('#loggedInSubsId').val() === "1") {
        if (isDropped === false || isDropped === 'false' || isDropped === null || isDropped === 'null') {
            var placeholderElement = $('#modal-placeholder');
            $.get('Canvas?handler=DR&COIId=' + coiId).done(function (data) {
                placeholderElement.html(data);
                modalDraggable();
                bindDropdowns();
                placeholderElement.find('#DropReportModal').modal('show');
            });
        }
        else {
            $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('This COI is already dropped.');
            $('#sweetAlertGridMessages').modal('show');
        }
    }
    else {
        $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('Only JMICC can create Drop Report.');
        $('#sweetAlertGridMessages').modal('show');
    }
}
function showAmpReport(coiId, mmsi, course, speed, isLost, isDropped) {
    if ($('#loggedInSubsId').val() === "1") {
        if (isDropped === false || isDropped === 'false' || isDropped === null || isDropped === 'null') {
            if (isLost === false || isLost === 'false' || isLost === null || isLost === 'null') {
                var placeholderElement = $('#modal-placeholder');
                $.get('Canvas?handler=AR&COIId=' + coiId).done(function (data) {
                    //console.log(data);
                    placeholderElement.html(data);
                    modalDraggable();
                    bindDropdowns();
                    if (mmsi === null && course === null && speed === null || mmsi === 'null' && course === 'null' && speed === 'null') {
                        $('.coiAmpReportTrackFields').hide();
                    }
                    placeholderElement.find('#AmplifyingReportModal').modal('show');
                });
            }
            else {
                $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('This COI is lost. So, you cannot create the Amplifying of it.');
                $('#sweetAlertGridMessages').modal('show');
            }
        }
        else {
            $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('This COI is dropped. So, you cannot create the Amplifying of it.');
            $('#sweetAlertGridMessages').modal('show');
        }
    }
    else {
        $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('Only JMICC can create Amplifying Report.');
        $('#sweetAlertGridMessages').modal('show');
    }
}
function showCOIActReportPR(prId, mmsi, course, speed, coiNumber, isLost, isDropped) {
    if ($('#loggedInSubsId').val() === "1") {
        if (isDropped === false || isDropped === 'false' || isDropped === null || isDropped === 'null') {
            if (isLost === false || isLost === 'false' || isLost === null || isLost === 'null') {
                if (coiNumber === 'null' || coiNumber === null) {
                    var placeholderElement = $('#modal-placeholder');
                    $.get('Canvas?handler=COI&PRId=' + prId).done(function (data) {
                        placeholderElement.html(data);
                        modalDraggable();
                        bindDropdowns();
                        if (mmsi === 'null' && course === 'null' && speed === 'null') {
                            $('.coiActReportTrackFields').hide();
                        }
                        placeholderElement.find('#COIActivationReportModal').modal('show');
                    });
                }
                else {
                    $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('COI is already generated against this Preliminary Report.');
                    $('#sweetAlertGridMessages').modal('show');
                }
            }
            else {
                $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('This Preliminary Report is lost. So, you cannot make it COI.');
                $('#sweetAlertGridMessages').modal('show');
            }
        }
        else {
            $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('This Preliminary Report is dropped. So, you cannot make it COI.');
            $('#sweetAlertGridMessages').modal('show');
        }
    }
    else {
        $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('Only JMICC is authorized to make it COI.');
        $('#sweetAlertGridMessages').modal('show');
    }
}
function showCOIActReportSR(srId, mmsi, course, speed, coiNumber) {
    if ($('#loggedInSubsId').val() === "1") {
        if (coiNumber === 'null' || coiNumber === null) {
            var placeholderElement = $('#modal-placeholder');
            $.get('Canvas?handler=COI&SRId=' + srId).done(function (data) {
                placeholderElement.html(data);
                modalDraggable();
                bindDropdowns();
                if (mmsi === 'null' && course === 'null' && speed === 'null') {
                    $('.coiActReportTrackFields').hide();
                }
                placeholderElement.find('#COIActivationReportModal').modal('show');
            });
        }
        else {
            $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('COI is already generated against this Subsequent Report.');
            $('#sweetAlertGridMessages').modal('show');
        }
    }
    else {
        $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('Only JMICC is authorized to make it COI.');
        $('#sweetAlertGridMessages').modal('show');
    }
}
function showSRReport(prId, mmsi, course, speed, coiNumber, isLost, isDropped, initiator) {
    if (initiator === $('#loggedInSubsCode').val() || $('#loggedInSubsId').val() === "1") {
        if (isDropped === 'false' || isDropped === false || isDropped === null || isDropped === 'null') {
            if (isLost === 'false' || isLost === false || isLost === null || isLost === 'null') {
                if (coiNumber === 'null' || coiNumber === null) {
                    var placeholderElement = $('#modal-placeholder');
                    $.get('Canvas?handler=SR&PRId=' + prId).done(function (data) {
                        placeholderElement.html(data);
                        bindDropdowns();
                        modalDraggable();
                        if (mmsi === 'null' && course === 'null' && speed === 'null') {
                            $('.subsReportTrackFields').hide();
                        }
                        $('#SubsequentReportModal').modal('show');
                    });
                }
                else {
                    $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('COI is already generated against this Preliminary Report.');
                    $('#sweetAlertGridMessages').modal('show');
                }
            }
            else {
                $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('This Preliminary Report is lost. So, you cannot create the Subsequent of it.');
                $('#sweetAlertGridMessages').modal('show');
            }
        }
        else {
            $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('This Preliminary Report is dropped. So, you cannot create the Subsequent of it.');
            $('#sweetAlertGridMessages').modal('show');
        }
    }
    else {
        $('#sweetAlertGridMessages').find('.modal-body #swal2-content').text('Only ' + initiator + ' can create Subsequent Report.');
        $('#sweetAlertGridMessages').modal('show');
    }
}

function viewPR(Id, mmsi, course, speed) {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=PR&Id=' + Id).done(function (data) {
        placeholderElement.html(data);
        bindDropdowns();
        modalDraggable();
        if (mmsi === 'null' && course === 'null' && speed === 'null') {
            $('.trackFields').hide();
        }
        $("#frmPreliminaryReport :input").prop("disabled", true);
        $('#PreliminaryReportModal').modal('show');
    });
}
function viewSR(Id, mmsi, course, speed) {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=SR&Id=' + Id).done(function (data) {
        placeholderElement.html(data);
        bindDropdowns();
        modalDraggable();
        if (mmsi === 'null' && course === 'null' && speed === 'null') {
            $('.subsReportTrackFields').hide();
        }
        $("#frmSubsequentReport :input").prop("disabled", true);
        $('#SubsequentReportModal').modal('show');
    });
}
function viewCOI(Id, mmsi, course, speed) {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=COI&Id=' + Id).done(function (data) {
        placeholderElement.html(data);
        bindDropdowns();
        modalDraggable();
        if (mmsi === null && course === null && speed === null) {
            $('.coiActReportTrackFields').hide();
        }
        $("#frmCOIActivation :input").prop("disabled", true);
        $('#COIActivationReportModal').modal('show');
    });
}
function viewAR(Id, mmsi, course, speed) {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=AR&Id=' + Id).done(function (data) {
        placeholderElement.html(data);
        bindDropdowns();
        modalDraggable();
        if (mmsi === null && course === null && speed === null || mmsi === 'null' && course === 'null' && speed === 'null') {
            $('.coiAmpReportTrackFields').hide();
        }
        $("#frmAmplifyingReport :input").prop("disabled", true);
        $('#AmplifyingReportModal').modal('show');
    });
}
function viewLR(Id, mmsi, course, speed) {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=LR&Id=' + Id).done(function (data) {
        placeholderElement.html(data);
        bindDropdowns();
        modalDraggable();
        if (mmsi === null && course === null && speed === null || mmsi === 'null' && course === 'null' && speed === 'null') {
            $('.lostReportTrackFields').hide();
        }
        $("#frmLostReport :input").prop("disabled", true);
        $('#LostReportModal').modal('show');
    });
}
function viewDR(Id) {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=DR&Id=' + Id).done(function (data) {
        placeholderElement.html(data);
        bindDropdowns();
        modalDraggable();
        $("#frmDropReport :input").prop("disabled", true);
        $('#DropReportModal').modal('show');
    });
}
function viewAAR(Id, immsi, icourse, ispeed, lmmsi, lcourse, lspeed) {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=AAR&Id=' + Id).done(function (data) {
        placeholderElement.html(data);
        bindDropdowns();
        modalDraggable();
        if (immsi === null && icourse === null && ispeed === null || immsi === 'null' && icourse === 'null' && ispeed === 'null') {
            $('.initialAARTrFields').hide();
        }
        if (lmmsi === null && lcourse === null && lspeed === null || lmmsi === 'null' && lcourse === 'null' && lspeed === 'null') {
            $('.lastAARTrFields').hide();
        }
        $("#frmAfterActionReport :input").prop("disabled", true);
        $('#AfterActionReportModal').modal('show');
    });
}
function viewTemplate(Id) {
    var placeholderElement = $('#modal-placeholder');
    $.get('Canvas?handler=Template&TemplateId=' + Id).done(function (data) {
        placeholderElement.html(data);
        bindDropdowns();
        modalDraggable();
        $("#frmTemplate :input").prop("disabled", true);
        $('#TemplateModal').modal('show');
    });
}

function viewReport(Id, Type) {
    var placeholderElement = $('#modal-placeholder');
    if (Type === "PR") {
        $.get('Canvas?handler=PR&Id=' + Id).done(function (data) {
            placeholderElement.html(data);
            bindDropdowns();
            modalDraggable();
            if ($('#mmsi').val() === '') {
                $('.trackFields').hide();
            }
            $("#frmPreliminaryReport :input").prop("disabled", true);
            $('#PreliminaryReportModal').modal('show');
        });
    }
    else if (Type === "SR") {
        $.get('Canvas?handler=SR&Id=' + Id).done(function (data) {
            placeholderElement.html(data);
            bindDropdowns();
            modalDraggable();
            if ($('#mmsi').val() === '') {
                $('.subsReportTrackFields').hide();
            }
            $("#frmSubsequentReport :input").prop("disabled", true);
            $('#SubsequentReportModal').modal('show');
        });
    }
    else if (Type === "COI") {
        $.get('Canvas?handler=COI&Id=' + Id).done(function (data) {
            placeholderElement.html(data);
            bindDropdowns();
            modalDraggable();
            if ($('#mmsi').val() === '') {
                $('.coiActReportTrackFields').hide();
            }
            $("#frmCOIActivation :input").prop("disabled", true);
            $('#COIActivationReportModal').modal('show');
        });
    }
    else if (Type === "AR") {
        $.get('Canvas?handler=AR&Id=' + Id).done(function (data) {
            placeholderElement.html(data);
            bindDropdowns();
            modalDraggable();
            if ($('#mmsi').val() === '') {
                $('.coiAmpReportTrackFields').hide();
            }
            $("#frmAmplifyingReport :input").prop("disabled", true);
            $('#AmplifyingReportModal').modal('show');
        });
    }
    else if (Type === "LR") {
        $.get('Canvas?handler=LR&Id=' + Id).done(function (data) {
            placeholderElement.html(data);
            bindDropdowns();
            modalDraggable();
            if ($('#mmsi').val() === '') {
                $('.lostReportTrackFields').hide();
            }
            $("#frmLostReport :input").prop("disabled", true);
            $('#LostReportModal').modal('show');
        });
    }
    else if (Type === "DR") {
        $.get('Canvas?handler=DR&Id=' + Id).done(function (data) {
            placeholderElement.html(data);
            bindDropdowns();
            modalDraggable();
            $("#frmDropReport :input").prop("disabled", true);
            $('#DropReportModal').modal('show');
        });
    }
    else if (Type === "AAR") {
        $.get('Canvas?handler=AAR&Id=' + Id).done(function (data) {
            placeholderElement.html(data);
            bindDropdowns();
            modalDraggable();
            if ($('#immsi').val() === '') {
                $('.initialAARTrFields').hide();
            }
            if ($('#lmmsi').val() === '') {
                $('.lastAARTrFields').hide();
            }
            $("#frmAfterActionReport :input").prop("disabled", true);
            $('#AfterActionReportModal').modal('show');
        });
    }
    else if (Type === "Template") {
        $.get('Canvas?handler=Template&TemplateId=' + Id).done(function (data) {
            placeholderElement.html(data);
            bindDropdowns();
            modalDraggable();
            $("#frmTemplate :input").prop("disabled", true);
            $('#TemplateModal').modal('show');
        });
    }
}

$('#TracksListModal').on('hidden.bs.modal', function (e) {
    $(".selected").removeClass("selected");
});

$('#PRGridModal').on('hidden.bs.modal', function (e) {
    prDatatable.destroy();
});

$('#SRGridModal').on('hidden.bs.modal', function (e) {
    srDatatable.destroy();
});

$('#COIGridModal').on('hidden.bs.modal', function (e) {
    coiDatatable.destroy();
});

$('#ARGridModal').on('hidden.bs.modal', function (e) {
    ARDatatable.destroy();
});

$('#LostContactGridModal').on('hidden.bs.modal', function (e) {
    LRDatatable.destroy();
});

$('#DropContactGridModal').on('hidden.bs.modal', function (e) {
    DRDatatable.destroy();
});

$('#AARGridModal').on('hidden.bs.modal', function (e) {
    AARDatatable.destroy();
});

$('#TemplateGridModal').on('hidden.bs.modal', function (e) {
    TemplateDatatable.destroy();
});

$('#noteGridModal').on('hidden.bs.modal', function (e) {
    NotesDatatable.destroy();
});

function enableInputs() {
    $('input:disabled, select:disabled').each(function () {
        $(this).removeAttr('disabled');
    });
}

function disableInputs() {
    $('input:disabled, select:disabled').each(function () {
        $(this).addAttr('disabled');
        console.log('Test');
    });
}

//#endregion Modals