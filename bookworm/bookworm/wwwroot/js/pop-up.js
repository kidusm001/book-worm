// Function to open the dialog
function openDialog() {
  var dialog = document.getElementById('loginDialog');
  dialog.showModal();
  document.body.classList.add('blur'); // Add blur class to body
  createBackdrop(); // Create backdrop
}

// Function to close the dialog
function closeDialog() {
  var dialog = document.getElementById('loginDialog');
  dialog.close();
  document.body.classList.remove('blur'); // Remove blur class from body
  removeBackdrop(); // Remove backdrop
}

// Function to create backdrop
function createBackdrop() {
  var backdrop = document.createElement('div');
  backdrop.className = 'dialog-backdrop';
  document.body.appendChild(backdrop);
}

// Function to remove backdrop
function removeBackdrop() {
  var backdrop = document.querySelector('.dialog-backdrop');
  if (backdrop) {
    backdrop.parentNode.removeChild(backdrop);
  }
}

// Listen for keydown event to close dialog with Escape key
document.addEventListener('keydown', function (event) {
  if (event.key === 'Escape') {
    closeDialog();
  }
});
