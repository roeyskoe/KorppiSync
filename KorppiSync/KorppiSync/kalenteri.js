function addScript( src ) {
    var s = document.createElement('script');
    s.setAttribute( 'src', src );
    document.body.appendChild(s );
}

addScript("https://cdn.jsdelivr.net/npm/table-to-json@1.0.0/lib/jquery.tabletojson.min.js");

function Lue() {
    let calendars = document.getElementsByClassName("calendartable");
    let data = [];
    for (let i = 0; i < calendars.length; i++) {
        let calendar = $(calendars[i]).tableToJSON({
            extractor: function (cellIndex, $cell) {
                return $cell.find('span').text() || $cell.text();
            }
        });
        data = data.concat(calendar);
    }
    return data;
}