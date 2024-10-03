
window.onSvgZoomOrResize = (dotnetHelper, elementId) => {
    const svgElement = document.getElementById(elementId);

    if (svgElement) {
        // Lắng nghe sự kiện zoom (thay đổi scale) của SVG
        svgElement.addEventListener('wheel', (event) => {
            // Scale factor là tỷ lệ thu nhỏ hoặc phóng to dựa trên chiều rộng SVG
            let scaleFactor = svgElement.getBoundingClientRect().width / svgElement.clientWidth;
            dotnetHelper.invokeMethodAsync("OnZoomOrResize", scaleFactor);
        });

        // Lắng nghe sự kiện thay đổi kích thước trình duyệt
        window.addEventListener('resize', () => {
            let width = svgElement.getBoundingClientRect().width;
            let height = svgElement.getBoundingClientRect().height;
            dotnetHelper.invokeMethodAsync("OnResize", width, height);
        });
    }
};
