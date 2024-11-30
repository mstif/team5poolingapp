$(document).ready(function () {
    if (typeof L !== "undefined") {
        var map = L.map("map").setView([55.677584, 37.683105], 13);
        L.tileLayer("https://tile.openstreetmap.org/{z}/{x}/{y}.png", {
            maxZoom: 19,
            attribution:
                '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
        }).addTo(map);

        //var marker = L.marker([55.677584, 37.683105]).addTo(map);
       // marker.bindPopup("<b>Hello world!</b><br>I am a popup.").openPopup();
        //var popup = L.popup()
        //    .setLatLng([55.6, 37.683105])
        //    .setContent("I am a standalone popup.")
        //    .openOn(map);
        map.on('click', onMapClick);
        $(".leaflet-attribution-flag").remove()

        if (typeof model !== "undefined") {
            $.each(model.Adresses, function (i, a) {
                let lon = parseFloat(a.Latitude);
                let lat = parseFloat(a.Longitude);
                let name = a.Name;
                let adress = a.Adress
                var markerp = L.marker([lat, lon]).addTo(map);
                markerp.bindPopup("<b>" + name + "</b><br>" + adress).openPopup();
            })
        }
    }
    var select = document.getElementById("adress")
    if (select != null) {
        select.addEventListener("change", function () {
            // Если нужно value
            // input.value = this.value;
            // Если нужен текст
            let position = this.value.split(" ");
            $("#latitude").val(position[0])
            $("#longitude").val(position[1])
            $('#geoadresshidd').val(this.options[this.selectedIndex].text);
            //input.value = this.options[this.selectedIndex].text;
        });
    }
    if (typeof modelContragent !== "undefined") {

    }
    $('#adress').append($('<option>', { value: $("#latitude").val() + " " + $("#longitude").val(), text: $('#geoadresshidd').val() }));
    
});
function onMapClick(e) {
    alert("You clicked the map at " + e.latlng);
}

function FillItemsAdress() {
    let adress = $("#region").val() + ', г. ' + $("#city").val() + ', ул. ' + $("#street").val() + ', д. ' + $("#buildnumber").val() + ', корп. ' + $("#buildnumber").val()
    GeoCode(adress);
};
function GeoCode(address) {
    //var address = 'Москва, ул. Тверская, д. 7';
    $.ajax({
        method: 'GET',
        url: 'https://geocode-maps.yandex.ru/1.x/?geocode=' + address + '&format=json&results=10&apikey=9cd192cd-2fd4-4f16-95b2-c3b67d917002'
    }).then(function (json) {
        var features = json.response.GeoObjectCollection.featureMember;
        if (!features)
            console.log('Не удалось распознать адрес');
        else {
            //console.log(feature.GeoObject.Point.pos)
            let res = features.map(function (x) {
                let tmp = {
                    adress: x.GeoObject.metaDataProperty.GeocoderMetaData.text,
                    position: x.GeoObject.Point.pos
                }
                return tmp;

            });
            $("#adress").find('option')
                .remove()
                .end();
            for (let i = 0; i < res.length; i++) {
                $('#adress').append($('<option>', { value: res[i].position, text: res[i].adress }));

            }
            if (res.length >= 1) {
                let pos = res[0].position.split(" ");
                $("#latitude").val(pos[0]);
                $("#longitude").val(pos[1]);
                $('#geoadresshidd').val(res[0].adress);
            }
 
        }
    });
}
function AdressToMap() {
    let lat = $("#latitude").val();
    let lon = $("#longitude").val();
    window.location = "openmap?Lat=" + lat + "&lon=" + lon + "&adress=" + $("#adress option:selected").text() + "&name=" + $("#name").val();
}


