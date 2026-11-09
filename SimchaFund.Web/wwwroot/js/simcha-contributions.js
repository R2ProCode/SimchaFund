$(() => {

    $("form").on('submit', function () {
        // const form = $(".update-form");
        // const id = $(this).data("id");
        // $("#simcha-id").val(id);
        console.log("What jQuery sees:", $(this).data());
        recalculateIndices();
    })

    let num = 1;
    function recalculateIndices() {
        let newIndex = 0;
        $(".contribution-row").each(function () {
            const row = $(this);

            row.find('.simcha-id').attr('name', `contributions[${newIndex}].SimchaId`);
            row.find('.contributor-id').attr('name', `contributions[${newIndex}].ContributorId`);
            row.find('.amount').attr('name', `contributions[${newIndex}].Amount`);
            console.log("What jQuery sees: hello world")
            newIndex++;
        })
        num = newIndex;
    }
});