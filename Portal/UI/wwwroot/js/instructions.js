var instructions = (function () {
    const instructionsListContainer = '#__InstructionsRows';

    function update(page) {
        load(page, false);
    }

    function load(page, paged) {
        let pg = page | 1;
        let pgd = paged;
        let canLoadMore = true;
        let totalRecords = 0;
        let activeRecords = 0;

        let requestParameters = pgd ? `?pageNumber=${pg}`:`?pageNumber=${pg}&paged=false`;
        $("#__lm_Instructions").removeClass("fa-chevron-down").addClass("fa-cog").addClass("fa-spin");
        $.ajax({
            url: `/data/instructions/${requestParameters}`,
            type: "GET",
            beforeSend: utility.setAccessToken,
            success: function (response) {
                if (!paged) {
                   $(instructionsListContainer).empty();
                }

                let instructions = response.results;
                totalRecords = response.total;

                $.each(instructions, function (index, result) {
                    let localDispatchedTime = utility.formatLongDate(result.dispatchTimeUTC );
                    let localSentTime = utility.formatLongDate(result.messageSentTimeUTC);
                    let ackWarn = '';
                    if (result.ackType === "ACKQ") {
                        ackWarn = "color-cellbg-warn";
                    }

                    $(instructionsListContainer).append(`
                            <tr class="hover-row" data-toggle="modal" data-target="#entityModal" onclick="instructions.details('${result.wtpDispatchInstructionID}')">
                                <td>${localDispatchedTime}</td>
                                <td>${localSentTime}</td>
                                <td>${result.sequenceNumber}</td>
                                <td>${result.node}</td>
                                <td>${result.primaryDispatchTypeCode}</td>
                                <td>${result.dispatchValue}</td>
                                <td class='${ackWarn}'>${result.ackType}</td> 
                               </tr>`);
                });

                activeRecords = $("#__InstructionsRows > .hover-row").length || 0;
                canLoadMore = activeRecords < totalRecords;
            },
            error: function (error) {
                notifications.addWarning("Warning", "Failed to fetch instructions", notifications.toastAndNotification)
            },
            complete: function () {
                $("#__lm_Instructions").removeClass("fa-spin").removeClass("fa-cog").addClass("fa-chevron-down");

                if (!canLoadMore) {
                    $("#__loadMoreContainer").fadeOut(100);
                }
            }
        });
    }

    function loadInstruction(dispatchInstructionID) {
        $("#_name").text("");
       
        $('#entityModalLabel').text("Loading...");
        $.ajax({
            url: `/data/instructions/${dispatchInstructionID}`,
            type: "GET",
            beforeSend: utility.setAccessToken,
            success: function (result) {
                $('#entityModalLabel').text(`${result.dispatchTypeDescription}`);
                $("#_WTPDispatchInstructionID ").text(`${result.wtpDispatchInstructionID}`);
                $("#_DispatchGroup").text(`${result.dispatchGroup}`);
                $("#_PrimaryDispatchTypeCode").text(`${result.primaryDispatchTypeCode}`);
                $("#_DispatchTypeDescription").text(`${result.dispatchTypeDescription}`);
                $("#_Node").text(`${result.node}`);
                $("#_CorrelationId").text(`${result.correlationId}`);
                $("#_SequenceNumber").text(`${result.sequenceNumber}`);
                $("#_DispatchEndpointOwner").text(`${result.dispatchEndpointOwner}`);
                $("#_IsUserResend").text(`${result.isUserResend}`);
                $("#_MessageSentTimeLocal").text(`${result.messageSentTimeLocal}`);
                $("#_MessageSentTimeUTC").text(`${result.messageSentTimeUTC}`);
                $("#_Tradercode").text(`${result.tradercode}`);
                $("#_DispatchValue").text(`${result.dispatchValue} MW`);
                $("#_DispatchTimeUTC").text(`${result.dispatchTimeUTC}`);
                $("#_DispatchTimeLocal").text(`${result.dispatchTimeLocal}`);
                $("#_DispatchIssueTimeUTC").text(`${result.dispatchIssueTimeUTC}`);
                $("#_DispatchIssueTimeLocal").text(`${result.dispatchIssueTimeLocal}`);
                $("#_AckType").text(`${result.ackType}`); 

            },
            error: function () {
                notifications.addError("Error", "Failed to load instructions", notifications.toastAndNotification);
            }
        });
    }   


    return {
        details: loadInstruction,
        load: load,
        update: update
    }
})();