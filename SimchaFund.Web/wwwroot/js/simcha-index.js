$(function () {
    $("#new-simcha").on("click", function () {
        new bootstrap.Modal($("#new-simcha-modal")[0]).show();
    });

    $("form").on('submit', function () {
        const form = $(".update-form");
        const id = $(this).data("id");
        $("#simcha-id").val(id);
        console.log("What jQuery sees:", $(this).data());
        recalculateIndices();
    })

    function recalculateIndices() {
        let newIndex = 0;
        $(".person-row").each(function () {
            const row = $(this);
            
            row.find('.simcha-id').attr('name', `contributions[${newIndex}].SimchaId`);
            row.find('.contributor-id').attr('name', `contributions[${newIndex}].ContributorId`);
            row.find('.amount').attr('name', `contributions[${newIndex}].Amount`);
            newIndex++;
        })
        num = newIndex;
    }
});
