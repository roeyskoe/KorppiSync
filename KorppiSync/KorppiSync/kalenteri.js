function Lue() {
    let calendar = document.getElementsByClassName("eventListTable");
    return calendar[0].tHead.innerText + calendar[0].tBodies[0].innerText;
}

Lue();