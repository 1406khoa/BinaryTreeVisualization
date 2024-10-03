function getSvgWidth(svgId) {
    const svgElement = document.getElementById(svgId);
    if (svgElement) {
        return svgElement.clientWidth;  // Trả về chiều rộng hiện tại của SVG
    }
    return 0;
}
