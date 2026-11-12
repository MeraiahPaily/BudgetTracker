// Confirm before deleting a transaction
document.querySelectorAll('.btn-delete').forEach(btn => {
    btn.addEventListener('click', function (e) {
        if (!confirm('Delete this transaction?')) {
            e.preventDefault();
        }
    });
});
