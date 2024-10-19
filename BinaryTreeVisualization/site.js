function revealTraversalStep(stepId) {
    console.log(`Attempting to reveal: ${stepId}`);
    var element = document.getElementById(stepId);

    if (element) {
        console.log(`Element found: ${stepId}`);
        element.hidden = false;
    } else {
        console.error(`Element not found: ${stepId}`);
    }
}
