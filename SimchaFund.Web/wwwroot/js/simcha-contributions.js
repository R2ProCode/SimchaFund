$(() => {

    $("form").on('submit', function () {

        console.log("What jQuery sees:", $(this).data());
        recalculateIndices();
    })

    //let num = 1;
    function recalculateIndices() {
        let newIndex = 0;
        $(".contribution-row").each(function () {
            const row = $(this);
            let IsIncluded = row.find('.form-check-input').is(':checked');
            if (IsIncluded) {
                row.find('.simcha-id').attr('name', `contributions[${newIndex}].SimchaId`);
                row.find('.contributor-id').attr('name', `contributions[${newIndex}].ContributorId`);
                row.find('.amount').attr('name', `contributions[${newIndex}].Amount`);
                console.log("What jQuery sees: hello world")
                newIndex++;
            } 
        })
        //num = newIndex;
    }
});