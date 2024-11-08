﻿if (typeof AnchorJS === 'function') {
    const anchors = new AnchorJS();
    document.addEventListener('DOMContentLoaded', function () {
        anchors.options.placement = "left";
        // Add anchors to h2, h3, h4, h5, and h6 elements, but exclude any with the "no-anchor" class.
        anchors.add('h2:not(.no-anchor), h3:not(.no-anchor), h4:not(.no-anchor), h5:not(.no-anchor), h6:not(.no-anchor)');
    })
}
