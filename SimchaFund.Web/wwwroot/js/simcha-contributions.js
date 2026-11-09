$(() => {

    $("form").on('submit', function () {

        console.log("What jQuery sees:", $(this).data());
        recalculateIndices();
    })


    function recalculateIndices() {
        let newIndex = 0;
        $(".contribution-row").each(function () {
            const row = $(this);
            let IsIncluded = row.find('.form-check-input').is(':checked');
            if (IsIncluded) {
            row.find('.simcha-id').attr('name', `contributors[${newIndex}].SimchaId`);
            row.find('.contributor-id').attr('name', `contributors[${newIndex}].ContributorId`);
            row.find('.amount').attr('name', `contributors[${newIndex}].Amount`);
            console.log("What jQuery sees:", $(this).data());
            newIndex++;
            }
        })
    }
});