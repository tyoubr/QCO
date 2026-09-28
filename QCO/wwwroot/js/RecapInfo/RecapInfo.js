// =========================================================
// RECAP INFO - 4 STEP CREATE / EDIT PAGE JS
// =========================================================

$(document).ready(function () {

    // =====================================================
    // VARIABLES
    // =====================================================

    let currentStep = 1;

    const totalSteps = 4;


    // =====================================================
    // SEWING FACTORIES
    // =====================================================

    const sewingFactories = [

        "Cotton Clothing BD Ltd.",

        "Tropical Knitex Ltd.",

        "Cotton Clout [BD] ltd",

        "Cotton Club BD ltd [Extended Part].",

        "Noor Checks & Stripes Ltd.",

        "Cotton Club BD Ltd."

    ];


    // =====================================================
    // SELECT2
    // =====================================================

    if ($.fn.select2) {

        $("#StyleName").select2({
            width: "100%",
            placeholder: "-- Select Style Name --",
            allowClear: true
        });


        $("#TeamLeaderName").select2({
            width: "100%",
            placeholder: "-- Select Team Leader --",
            allowClear: true
        });


        $("#BookingNo").select2({
            width: "100%",
            placeholder: "-- Select Internal Ref. --",
            allowClear: true
        });


        $("#itemNameInput").select2({
            width: "100%",
            placeholder: "-- Select Item Name --",
            allowClear: true
        });

    }


    // =====================================================
    // SHOW STEP
    // =====================================================

    function showStep(step) {

        // -------------------------------------------------
        // Safety
        // -------------------------------------------------

        if (step < 1) {
            step = 1;
        }

        if (step > totalSteps) {
            step = totalSteps;
        }


        currentStep = step;


        // -------------------------------------------------
        // Hide all
        // -------------------------------------------------

        $(".step-content").hide();


        // -------------------------------------------------
        // Show current
        // -------------------------------------------------

        $("#step" + step).show();


        // -------------------------------------------------
        // Reset stepper
        // -------------------------------------------------

        $(".step-wrapper")
            .removeClass("active completed");


        $(".step-line")
            .removeClass("active");


        // -------------------------------------------------
        // Step 1
        // -------------------------------------------------

        if (step === 1) {

            $("#step1Wrapper")
                .addClass("active");

        }
        else {

            $("#step1Wrapper")
                .addClass("completed");

            $("#stepLine1")
                .addClass("active");
        }


        // -------------------------------------------------
        // Step 2
        // -------------------------------------------------

        if (step === 2) {

            $("#step2Wrapper")
                .addClass("active");

        }
        else if (step > 2) {

            $("#step2Wrapper")
                .addClass("completed");

            $("#stepLine2")
                .addClass("active");
        }


        // -------------------------------------------------
        // Step 3
        // -------------------------------------------------

        if (step === 3) {

            $("#step3Wrapper")
                .addClass("active");

            $("#stepLine2")
                .addClass("active");


            // ---------------------------------------------
            // Load sewing assignment
            // ---------------------------------------------

            loadSewingAssignmentItems();

        }
        else if (step > 3) {

            $("#step3Wrapper")
                .addClass("completed");

            $("#stepLine3")
                .addClass("active");
        }


        // -------------------------------------------------
        // Step 4
        // -------------------------------------------------

        if (step === 4) {

            $("#step4Wrapper")
                .addClass("active");
        }


        console.log(
            "Current Step:",
            currentStep
        );

    }


    // =====================================================
    // INITIAL STEP
    // =====================================================

    showStep(1);


    // =====================================================
    // STYLE -> BUYER
    // =====================================================

    $(document).on(
        "change",
        "#StyleName",
        function () {

            const buyerName =
                $(this)
                    .find("option:selected")
                    .data("buyer") || "";


            $("#BuyerName")
                .val(buyerName);

        }
    );


    // =====================================================
    // ADD ITEM
    // =====================================================

    $("#addItem").on(
        "click",
        function () {

            const itemName =
                $("#itemNameInput").val();


            const offeredQty =
                $("#offeredQtyInput").val();


            const quotedPrice =
                $("#quotedPriceInput").val();


            const remarks =
                $("#remarksInput").val();


            const isPrint =
                $("#isPrintInput").is(":checked")
                    ? 1
                    : 0;


            const isWash =
                $("#isWashInput").is(":checked")
                    ? 1
                    : 0;


            const isEmb =
                $("#isEmbInput").is(":checked")
                    ? 1
                    : 0;


            // -------------------------------------------------
            // Validation
            // -------------------------------------------------

            if (!itemName) {

                alert(
                    "Please select Item Name."
                );

                return;
            }


            if (
                offeredQty === "" ||
                offeredQty === null ||
                offeredQty === undefined
            ) {

                alert(
                    "Please enter Order Qty."
                );

                return;
            }


            if (parseFloat(offeredQty) <= 0) {

                alert(
                    "Order Qty. must be greater than 0."
                );

                return;
            }


            if (
                quotedPrice === "" ||
                quotedPrice === null ||
                quotedPrice === undefined
            ) {

                alert(
                    "Please enter Quoted Price."
                );

                return;
            }


            // -------------------------------------------------
            // Serial
            // -------------------------------------------------

            const serial =
                $("#itemsBody tr").length + 1;


            // -------------------------------------------------
            // Display
            // -------------------------------------------------

            const printText =
                isPrint === 1
                    ? "Yes"
                    : "No";


            const washText =
                isWash === 1
                    ? "Yes"
                    : "No";


            const embText =
                isEmb === 1
                    ? "Yes"
                    : "No";


            // -------------------------------------------------
            // Row
            // -------------------------------------------------

            const row = `

                <tr

                    data-item-name="${escapeHtmlAttribute(itemName)}"

                    data-offered-qty="${escapeHtmlAttribute(offeredQty)}"

                    data-quoted-price="${escapeHtmlAttribute(quotedPrice)}"

                    data-is-print="${isPrint}"

                    data-is-wash="${isWash}"

                    data-is-emb="${isEmb}"

                    data-remarks="${escapeHtmlAttribute(remarks)}"

                >

                    <td class="text-center align-middle">
                        ${serial}
                    </td>


                    <td class="text-center align-middle item-name-cell">
                        ${escapeHtml(itemName)}
                    </td>


                    <td class="text-center align-middle">
                        ${escapeHtml(offeredQty)}
                    </td>


                    <td class="text-center align-middle">
                        ${escapeHtml(quotedPrice)}
                    </td>


                    <td class="text-center align-middle">
                        ${printText}
                    </td>


                    <td class="text-center align-middle">
                        ${washText}
                    </td>


                    <td class="text-center align-middle">
                        ${embText}
                    </td>


                    <td class="text-center align-middle remarks-cell">
                        ${escapeHtml(remarks)}
                    </td>


                    <td class="text-center align-middle">

                        <button
                            type="button"
                            class="btn btn-sm btn-danger delete-item">

                            Remove

                        </button>

                    </td>

                </tr>

            `;


            $("#itemsBody")
                .append(row);


            // -------------------------------------------------
            // Clear inputs
            // -------------------------------------------------

            $("#itemNameInput")
                .val("")
                .trigger("change");


            $("#offeredQtyInput")
                .val("");


            $("#quotedPriceInput")
                .val("");


            $("#remarksInput")
                .val("");


            $("#isPrintInput")
                .prop("checked", false);


            $("#isWashInput")
                .prop("checked", false);


            $("#isEmbInput")
                .prop("checked", false);


            // -------------------------------------------------
            // Hidden inputs
            // -------------------------------------------------

            buildItemHiddenInputs();


            // -------------------------------------------------
            // Renumber
            // -------------------------------------------------

            renumberItems();

        }
    );


    // =====================================================
    // DELETE ITEM
    // =====================================================

    $(document).on(
        "click",
        ".delete-item",
        function () {

            $(this)
                .closest("tr")
                .remove();


            renumberItems();


            buildItemHiddenInputs();

        }
    );


    // =====================================================
    // RENUMBER ITEMS
    // =====================================================

    function renumberItems() {

        $("#itemsBody tr")
            .each(
                function (index) {

                    $(this)
                        .find("td:first")
                        .text(index + 1);

                }
            );

    }


    // =====================================================
    // BUILD ITEM HIDDEN INPUTS
    // =====================================================

    function buildItemHiddenInputs() {

        const $container =
            $("#itemDetailsInputs");


        if (!$container.length) {

            console.error(
                "#itemDetailsInputs not found."
            );

            return;
        }


        $container.empty();


        $("#itemsBody tr")
            .each(
                function (index) {

                    const $row =
                        $(this);


                    const itemName =
                        $row.attr("data-item-name") || "";


                    const offeredQty =
                        $row.attr("data-offered-qty") || "";


                    const quotedPrice =
                        $row.attr("data-quoted-price") || "";


                    const remarks =
                        $row.attr("data-remarks") || "";


                    const isPrint =
                        parseInt(
                            $row.attr("data-is-print") || "0",
                            10
                        );


                    const isWash =
                        parseInt(
                            $row.attr("data-is-wash") || "0",
                            10
                        );


                    const isEmb =
                        parseInt(
                            $row.attr("data-is-emb") || "0",
                            10
                        );


                    appendHiddenInput(
                        $container,
                        `ItemDetails[${index}].ItemName`,
                        itemName
                    );


                    appendHiddenInput(
                        $container,
                        `ItemDetails[${index}].OfferedQty`,
                        offeredQty
                    );


                    appendHiddenInput(
                        $container,
                        `ItemDetails[${index}].QuoatedPrice`,
                        quotedPrice
                    );


                    appendHiddenInput(
                        $container,
                        `ItemDetails[${index}].Remarks`,
                        remarks
                    );


                    appendHiddenInput(
                        $container,
                        `ItemDetails[${index}].IsPrint`,
                        isPrint
                    );


                    appendHiddenInput(
                        $container,
                        `ItemDetails[${index}].IsWash`,
                        isWash
                    );


                    appendHiddenInput(
                        $container,
                        `ItemDetails[${index}].IsEmb`,
                        isEmb
                    );

                }
            );

    }


    // =====================================================
    // APPEND HIDDEN INPUT
    // =====================================================

    function appendHiddenInput(
        $container,
        name,
        value
    ) {

        $("<input>", {

            type: "hidden",

            name: name,

            value:
                value === null ||
                    value === undefined
                    ? ""
                    : value

        })
            .appendTo($container);

    }


    // =====================================================
    // STEP 1 -> STEP 2
    // =====================================================

    $("#nextStep1").on(
        "click",
        function () {

            const styleName =
                $("#StyleName").val();


            if (!styleName) {

                alert(
                    "Please select Style Name."
                );

                return;
            }


            const itemCount =
                $("#itemsBody tr").length;


            if (itemCount === 0) {

                alert(
                    "Please add at least one Item."
                );

                return;
            }


            buildItemHiddenInputs();


            showStep(2);

        }
    );


    // =====================================================
    // BOOKING / INTERNAL REF CHANGE
    // =====================================================

    $("#BookingNo").on(
        "change",
        function () {

            const bookingNo =
                $(this).val();


            $("#PoNo")
                .val("");


            $("#detailsBody")
                .empty();


            $("#recapDetailsInputs")
                .empty();


            $("#noDetailsMessage")
                .hide();


            if (!bookingNo) {

                return;
            }


            $("#detailsBody")
                .html(`

                    <tr>

                        <td colspan="9"
                            class="text-center">

                            Loading Recap Details...

                        </td>

                    </tr>

                `);


            $("#BookingNo")
                .prop("disabled", true);


            $.ajax({

                url:
                    "/RecapeInfo/GetDetailsByBookingNo",

                type:
                    "GET",

                data:
                {
                    bookingNo:
                        bookingNo
                },

                dataType:
                    "json",


                success:
                    function (response) {

                        console.log(
                            "GetDetailsByBookingNo response:",
                            response
                        );


                        $("#detailsBody")
                            .empty();


                        $("#recapDetailsInputs")
                            .empty();


                        $("#noDetailsMessage")
                            .hide();


                        if (!response) {

                            alert(
                                "Failed to load Recap Details."
                            );

                            return;
                        }


                        if (
                            response.success === false
                        ) {

                            alert(
                                response.message ||
                                "Failed to load Recap Details."
                            );

                            return;
                        }


                        $("#PoNo")
                            .val(
                                response.poNo || ""
                            );


                        const data =
                            Array.isArray(response.data)
                                ? response.data
                                : [];


                        if (data.length === 0) {

                            $("#noDetailsMessage")
                                .show();


                            $("#detailsBody")
                                .html(`

                                    <tr>

                                        <td colspan="9"
                                            class="text-center text-muted">

                                            No recap details found for this Internal Ref.

                                        </td>

                                    </tr>

                                `);

                            return;
                        }


                        data.forEach(
                            function (item, index) {

                                const itemName =
                                    item.itemName ?? "";


                                const bodyPart =
                                    item.bodyPart ?? "";


                                const fabrication =
                                    item.fabrication ?? "";


                                const gsm =
                                    item.gsm ?? "";


                                const color =
                                    item.color ?? "";


                                const consPcs =
                                    item.consPcs ?? "";


                                const totalQty =
                                    item.totalQty ?? "";


                                const row = `

                                    <tr

                                        data-item-name="${escapeHtmlAttribute(itemName)}"

                                        data-body-part="${escapeHtmlAttribute(bodyPart)}"

                                        data-fabrication="${escapeHtmlAttribute(fabrication)}"

                                        data-gsm="${escapeHtmlAttribute(gsm)}"

                                        data-color="${escapeHtmlAttribute(color)}"

                                        data-cons-pcs="${escapeHtmlAttribute(consPcs)}"

                                        data-total-qty="${escapeHtmlAttribute(totalQty)}"

                                    >

                                        <td class="text-center align-middle">
                                            ${index + 1}
                                        </td>


                                        <td class="text-center align-middle">
                                            ${escapeHtml(itemName)}
                                        </td>


                                        <td class="text-center align-middle">
                                            ${escapeHtml(bodyPart)}
                                        </td>


                                        <td class="text-center align-middle">
                                            ${escapeHtml(fabrication)}
                                        </td>


                                        <td class="text-center align-middle">
                                            ${escapeHtml(gsm)}
                                        </td>


                                        <td class="text-center align-middle">
                                            ${escapeHtml(color)}
                                        </td>


                                        <td class="text-center align-middle">
                                            ${escapeHtml(consPcs)}
                                        </td>


                                        <td class="text-center align-middle">
                                            ${escapeHtml(totalQty)}
                                        </td>


                                        <td class="text-center align-middle">

                                            <button
                                                type="button"
                                                class="btn btn-sm btn-danger delete-recap-detail">

                                                Remove

                                            </button>

                                        </td>

                                    </tr>

                                `;


                                $("#detailsBody")
                                    .append(row);

                            }
                        );


                        buildRecapDetailHiddenInputsFromTable();

                    },


                error:
                    function (xhr) {

                        console.error(
                            "GetDetailsByBookingNo ERROR:",
                            xhr
                        );


                        $("#detailsBody")
                            .empty();


                        $("#recapDetailsInputs")
                            .empty();


                        alert(
                            "Failed to load Recap Details."
                        );

                    },


                complete:
                    function () {

                        $("#BookingNo")
                            .prop(
                                "disabled",
                                false
                            );

                    }

            });

        }
    );


    // =====================================================
    // DELETE RECAP DETAIL
    // =====================================================

    $(document).on(
        "click",
        ".delete-recap-detail",
        function () {

            $(this)
                .closest("tr")
                .remove();


            renumberRecapDetails();


            buildRecapDetailHiddenInputsFromTable();


            const remainingRows =
                $("#detailsBody tr")
                    .filter(function () {

                        return $(this)
                            .find(".delete-recap-detail")
                            .length > 0;

                    })
                    .length;


            if (remainingRows === 0) {

                $("#noDetailsMessage")
                    .show();


                $("#detailsBody")
                    .html(`

                        <tr>

                            <td colspan="9"
                                class="text-center text-muted">

                                No Recap Details available.

                            </td>

                        </tr>

                    `);


                $("#recapDetailsInputs")
                    .empty();

            }

        }
    );


    // =====================================================
    // RENUMBER RECAP DETAILS
    // =====================================================

    function renumberRecapDetails() {

        $("#detailsBody tr")
            .each(
                function (index) {

                    $(this)
                        .find("td:first")
                        .text(index + 1);

                }
            );

    }


    // =====================================================
    // BUILD RECAP DETAIL HIDDEN INPUTS
    // =====================================================

    function buildRecapDetailHiddenInputsFromTable() {

        const $container =
            $("#recapDetailsInputs");


        if (!$container.length) {
            return;
        }


        $container.empty();


        let realIndex = 0;


        $("#detailsBody tr")
            .each(
                function () {

                    const $row =
                        $(this);


                    if (
                        !$row.find(
                            ".delete-recap-detail"
                        ).length
                    ) {

                        return;
                    }


                    const itemName =
                        $row.attr("data-item-name") || "";


                    const bodyPart =
                        $row.attr("data-body-part") || "";


                    const fabrication =
                        $row.attr("data-fabrication") || "";


                    const gsm =
                        $row.attr("data-gsm") || "";


                    const color =
                        $row.attr("data-color") || "";


                    const consPcs =
                        $row.attr("data-cons-pcs") || "";


                    const totalQty =
                        $row.attr("data-total-qty") || "";


                    appendHiddenInput(
                        $container,
                        `Details[${realIndex}].ItemName`,
                        itemName
                    );


                    appendHiddenInput(
                        $container,
                        `Details[${realIndex}].BodyPart`,
                        bodyPart
                    );


                    appendHiddenInput(
                        $container,
                        `Details[${realIndex}].Fabrication`,
                        fabrication
                    );


                    appendHiddenInput(
                        $container,
                        `Details[${realIndex}].Gsm`,
                        gsm
                    );


                    appendHiddenInput(
                        $container,
                        `Details[${realIndex}].ColorName`,
                        color
                    );


                    appendHiddenInput(
                        $container,
                        `Details[${realIndex}].ConsPerUnit`,
                        consPcs
                    );


                    appendHiddenInput(
                        $container,
                        `Details[${realIndex}].TotalQty`,
                        totalQty
                    );


                    realIndex++;

                }
            );

    }


    // =====================================================
    // STEP 2 -> STEP 3
    // =====================================================

    $("#nextStep2").on(
        "click",
        function () {

            const bookingNo =
                $("#BookingNo").val();


            if (bookingNo) {

                buildRecapDetailHiddenInputsFromTable();


                const detailCount =
                    $("#detailsBody tr")
                        .filter(function () {

                            return $(this)
                                .find(".delete-recap-detail")
                                .length > 0;

                        })
                        .length;


                if (detailCount === 0) {

                    alert(
                        "No Recap Detail found for the selected Internal Ref."
                    );

                    return;
                }

            }


            // -------------------------------------------------
            // Go Step 3
            // -------------------------------------------------

            showStep(3);

        }
    );


    // =====================================================
    // STEP 2 -> STEP 1
    // =====================================================

    $("#previousStep2").on(
        "click",
        function () {

            showStep(1);

        }
    );


    // =====================================================
    // STEP 3 -> STEP 2
    // =====================================================

    $("#previousStep3").on(
        "click",
        function () {

            showStep(2);

        }
    );


    // =====================================================
    // LOAD SEWING ASSIGNMENT ITEMS
    // =====================================================

    function loadSewingAssignmentItems() {

        const container =
            document.getElementById(
                "sewingAssignmentContainer"
            );


        const itemsBody =
            document.getElementById(
                "itemsBody"
            );


        if (!container || !itemsBody) {

            console.error(
                "Sewing assignment container/items body not found."
            );

            return;
        }


        container.innerHTML = "";


        const rows =
            itemsBody.querySelectorAll("tr");


        if (rows.length === 0) {

            container.innerHTML = `

                <div class="alert alert-warning">

                    No items found from Step 1.

                    Please go back to Step 1
                    and add at least one item.

                </div>

            `;

            return;
        }


        rows.forEach(
            function (row, index) {

                const itemName =
                    row.getAttribute(
                        "data-item-name"
                    ) || "";


                const offeredQty =
                    parseFloat(
                        row.getAttribute(
                            "data-offered-qty"
                        )
                    ) || 0;


                createSewingItemCard(
                    container,
                    index,
                    itemName,
                    offeredQty
                );

            }
        );


        refreshAllSewingCalculations();

    }


    // =====================================================
    // CREATE ITEM CARD
    // =====================================================

    function createSewingItemCard(
        container,
        itemIndex,
        itemName,
        offeredQty
    ) {

        const card =
            document.createElement("div");


        card.className =
            "sewing-item-card";


        card.dataset.itemIndex =
            itemIndex;


        card.dataset.itemName =
            itemName;


        card.dataset.totalQty =
            offeredQty;


        card.innerHTML = `

            <!-- =========================================
                 ITEM HEADER
                 ========================================= -->

            <div class="sewing-item-header">

                <div class="d-flex
                            justify-content-between
                            align-items-center
                            flex-wrap
                            gap-2">


                    <div>

                        <div class="sewing-item-title">

                            ${escapeHtml(itemName)}

                        </div>

                    </div>


                    <div class="sewing-qty-summary">


                        <div class="sewing-qty-box">

                            <span class="sewing-qty-label">
                                Order Qty
                            </span>

                            <span class="sewing-qty-value">

                                ${formatQty(offeredQty)}

                            </span>

                        </div>


                        <div class="sewing-qty-box">

                            <span class="sewing-qty-label">
                                Assigned
                            </span>

                            <span
                                class="sewing-qty-value
                                       sewing-assigned-value"
                                data-role="assignedQty">

                                0

                            </span>

                        </div>


                        <div class="sewing-qty-box">

                            <span class="sewing-qty-label">
                                Remaining
                            </span>

                            <span
                                class="sewing-qty-value
                                       sewing-remaining-value"
                                data-role="remainingQty">

                                ${formatQty(offeredQty)}

                            </span>

                        </div>


                        <span
                            class="sewing-status-badge
                                   sewing-status-pending"
                            data-role="status">

                            Pending

                        </span>

                    </div>

                </div>

            </div>


            <!-- =========================================
                 ITEM BODY
                 ========================================= -->

            <div class="sewing-item-body">


                <div class="table-responsive">


                    <table class="table
                                  table-sm
                                  sewing-assignment-table">


                        <thead>

                            <tr>

                                <th style="width:30%;">
                                    Sewing Factory
                                </th>


                                <th style="width:16%;">
                                    Assign Qty.
                                </th>


                                <th style="width:12%;">
                                    SMV
                                </th>


                                <th style="width:12%;">
                                    Machine
                                </th>


                                <th style="width:24%;">
                                    Remarks
                                </th>


                                <th style="width:6%;">
                                </th>

                            </tr>

                        </thead>


                        <tbody
                            data-role="assignmentRows">
                        </tbody>


                    </table>

                </div>


                <button
                    type="button"
                    class="btn btn-sm sewing-add-btn"
                    data-role="addFactory">

                    + Add Factory

                </button>


            </div>

        `;


        container.appendChild(card);


        // -------------------------------------------------
        // Add Factory
        // -------------------------------------------------

        const addButton =
            card.querySelector(
                '[data-role="addFactory"]'
            );


        addButton.addEventListener(
            "click",
            function () {

                addSewingFactoryRow(card);

            }
        );


        // -------------------------------------------------
        // Initial row
        // -------------------------------------------------

        addSewingFactoryRow(card);

    }


    // =====================================================
    // ADD FACTORY ROW
    // =====================================================

    function addSewingFactoryRow(card) {

        const tbody =
            card.querySelector(
                '[data-role="assignmentRows"]'
            );


        if (!tbody) {
            return;
        }


        // -------------------------------------------------
        // Check Remaining Qty
        // -------------------------------------------------

        const totalQty =
            parseFloat(
                card.dataset.totalQty
            ) || 0;


        const assignedQty =
            calculateAssignedQty(card);


        const remainingQty =
            totalQty - assignedQty;


        if (remainingQty <= 0) {

            updateAddFactoryButton(card);

            return;
        }


        // -------------------------------------------------
        // Row
        // -------------------------------------------------

        const row =
            document.createElement("tr");


        row.innerHTML = `

            <td>

                <select
                    class="form-select sewing-factory">

                    <option value="">
                        -- Select Factory --
                    </option>

                    ${sewingFactories.map(
            function (factory) {

                return `

                                <option
                                    value="${escapeHtmlAttribute(factory)}">

                                    ${escapeHtml(factory)}

                                </option>

                            `;

            }
        ).join("")}

                </select>

            </td>


            <td>

                <input
                    type="number"
                    class="form-control sewing-production-qty"
                    min="0"
                    step="0.01"
                    placeholder="0">

            </td>


            <td>

                <input
                    type="number"
                    class="form-control sewing-smv"
                    min="0"
                    step="0.01"
                    placeholder="SMV">

            </td>


            <td>

                <input
                    type="number"
                    class="form-control sewing-machine"
                    min="0"
                    step="1"
                    placeholder="Machine">

            </td>


            <td>

                <input
                    type="text"
                    class="form-control sewing-row-remarks"
                    placeholder="Remarks">

            </td>


            <td class="text-center">

                <button
                    type="button"
                    class="btn btn-outline-danger btn-sm sewing-delete-btn"
                    title="Remove">

                    ×

                </button>

            </td>

        `;


        tbody.appendChild(row);


        // =================================================
        // EVENTS
        // =================================================

        const qtyInput =
            row.querySelector(
                ".sewing-production-qty"
            );


        const factorySelect =
            row.querySelector(
                ".sewing-factory"
            );


        const deleteButton =
            row.querySelector(
                ".sewing-delete-btn"
            );


        // -------------------------------------------------
        // Quantity input
        // -------------------------------------------------

        qtyInput.addEventListener(
            "input",
            function () {

                handleSewingQuantityInput(
                    card,
                    this
                );

            }
        );


        // -------------------------------------------------
        // Factory change
        // -------------------------------------------------

        factorySelect.addEventListener(
            "change",
            function () {

                validateSewingFactories(card);

            }
        );


        // -------------------------------------------------
        // Delete
        // -------------------------------------------------

        deleteButton.addEventListener(
            "click",
            function () {

                row.remove();


                validateSewingQuantity(card);

                validateSewingFactories(card);

                updateAddFactoryButton(card);

            }
        );


        validateSewingQuantity(card);

        validateSewingFactories(card);

        updateAddFactoryButton(card);

    }


    // =====================================================
    // HANDLE QTY INPUT
    // =====================================================

    function handleSewingQuantityInput(
        card,
        input
    ) {

        const totalQty =
            parseFloat(
                card.dataset.totalQty
            ) || 0;


        // -------------------------------------------------
        // Calculate other rows
        // -------------------------------------------------

        let otherAssignedQty = 0;


        card.querySelectorAll(
            ".sewing-production-qty"
        ).forEach(
            function (qtyInput) {

                if (qtyInput === input) {
                    return;
                }


                const qty =
                    parseFloat(
                        qtyInput.value
                    ) || 0;


                otherAssignedQty += qty;

            }
        );


        // -------------------------------------------------
        // Maximum allowed for this row
        // -------------------------------------------------

        const maxAllowed =
            Math.max(
                totalQty - otherAssignedQty,
                0
            );


        let currentQty =
            parseFloat(
                input.value
            );


        if (isNaN(currentQty)) {
            currentQty = 0;
        }


        // -------------------------------------------------
        // Prevent negative
        // -------------------------------------------------

        if (currentQty < 0) {

            input.value = 0;

            currentQty = 0;

        }


        // -------------------------------------------------
        // Prevent over quantity
        // -------------------------------------------------

        if (currentQty > maxAllowed) {

            input.value =
                maxAllowed;


            input.classList.add(
                "sewing-invalid"
            );


            showSewingQtyWarning(
                card,
                totalQty
            );


            setTimeout(
                function () {

                    input.classList.remove(
                        "sewing-invalid"
                    );

                },
                1000
            );

        }


        // -------------------------------------------------
        // Recalculate
        // -------------------------------------------------

        validateSewingQuantity(card);


        updateAddFactoryButton(card);

    }


    // =====================================================
    // QTY WARNING
    // =====================================================

    function showSewingQtyWarning(
        card,
        totalQty
    ) {

        const itemName =
            card.dataset.itemName || "Item";


        const assignedQty =
            calculateAssignedQty(card);


        const remainingQty =
            Math.max(
                totalQty - assignedQty,
                0
            );


        alert(
            "Assigned Qty. cannot be greater than Offered Qty.\n\n" +
            "Item: " + itemName + "\n" +
            "Order Qty: " + formatQty(totalQty) + "\n" +
            "Already Assigned: " + formatQty(assignedQty) + "\n" +
            "Remaining Qty: " + formatQty(remainingQty)
        );

    }


    // =====================================================
    // CALCULATE ASSIGNED QTY
    // =====================================================

    function calculateAssignedQty(card) {

        let assignedQty = 0;


        card.querySelectorAll(
            ".sewing-production-qty"
        ).forEach(
            function (input) {

                const qty =
                    parseFloat(
                        input.value
                    ) || 0;


                assignedQty += qty;

            }
        );


        return assignedQty;

    }


    // =====================================================
    // VALIDATE SEWING QUANTITY
    // =====================================================

    function validateSewingQuantity(card) {

        const totalQty =
            parseFloat(
                card.dataset.totalQty
            ) || 0;


        const assignedQty =
            calculateAssignedQty(card);


        const remainingQty =
            Math.max(
                totalQty - assignedQty,
                0
            );


        const assignedElement =
            card.querySelector(
                '[data-role="assignedQty"]'
            );


        const remainingElement =
            card.querySelector(
                '[data-role="remainingQty"]'
            );


        const statusElement =
            card.querySelector(
                '[data-role="status"]'
            );


        if (assignedElement) {

            assignedElement.innerText =
                formatQty(assignedQty);

        }


        if (remainingElement) {

            remainingElement.innerText =
                formatQty(remainingQty);

        }


        // -------------------------------------------------
        // Remove status classes
        // -------------------------------------------------

        statusElement.classList.remove(

            "sewing-status-complete",

            "sewing-status-partial",

            "sewing-status-pending",

            "sewing-status-over"

        );


        remainingElement.classList.remove(
            "sewing-remaining-zero"
        );


        // -------------------------------------------------
        // Complete
        // -------------------------------------------------

        if (
            assignedQty === totalQty &&
            totalQty > 0
        ) {

            remainingElement.classList.add(
                "sewing-remaining-zero"
            );


            statusElement.classList.add(
                "sewing-status-complete"
            );


            statusElement.innerText =
                "Complete";

        }


        // -------------------------------------------------
        // Partial
        // -------------------------------------------------

        else if (
            assignedQty > 0 &&
            assignedQty < totalQty
        ) {

            statusElement.classList.add(
                "sewing-status-partial"
            );


            statusElement.innerText =
                "Partial";

        }


        // -------------------------------------------------
        // Pending
        // -------------------------------------------------

        else {

            statusElement.classList.add(
                "sewing-status-pending"
            );


            statusElement.innerText =
                "Pending";

        }


        // -------------------------------------------------
        // Over Qty
        // -------------------------------------------------

        if (assignedQty > totalQty) {

            statusElement.classList.remove(
                "sewing-status-pending",
                "sewing-status-partial"
            );


            statusElement.classList.add(
                "sewing-status-over"
            );


            statusElement.innerText =
                "Over Qty";

        }


        // -------------------------------------------------
        // Update button
        // -------------------------------------------------

        updateAddFactoryButton(card);

    }


    // =====================================================
    // ADD FACTORY BUTTON ENABLE / DISABLE
    // =====================================================

    function updateAddFactoryButton(card) {

        const addButton =
            card.querySelector(
                '[data-role="addFactory"]'
            );


        if (!addButton) {
            return;
        }


        const totalQty =
            parseFloat(
                card.dataset.totalQty
            ) || 0;


        const assignedQty =
            calculateAssignedQty(card);


        // -------------------------------------------------
        // Full Qty Assigned
        // -------------------------------------------------

        if (
            assignedQty >= totalQty &&
            totalQty > 0
        ) {

            addButton.disabled = true;


            addButton.classList.add(
                "disabled"
            );


            addButton.innerText =
                "✓ Qty Fully Assigned";

        }


        // -------------------------------------------------
        // Remaining Qty
        // -------------------------------------------------

        else {

            addButton.disabled = false;


            addButton.classList.remove(
                "disabled"
            );


            addButton.innerText =
                "+ Add Factory";

        }

    }


    // =====================================================
    // DUPLICATE FACTORY VALIDATION
    // =====================================================

    function validateSewingFactories(card) {

        const selects =
            card.querySelectorAll(
                ".sewing-factory"
            );


        const selectedFactories = [];


        selects.forEach(
            function (select) {

                select.classList.remove(
                    "sewing-invalid"
                );


                if (!select.value) {
                    return;
                }


                if (
                    selectedFactories.includes(
                        select.value
                    )
                ) {

                    select.classList.add(
                        "sewing-invalid"
                    );

                }
                else {

                    selectedFactories.push(
                        select.value
                    );

                }

            }
        );

    }


    // =====================================================
    // REFRESH ALL SEWING CALCULATIONS
    // =====================================================

    function refreshAllSewingCalculations() {

        document
            .querySelectorAll(
                ".sewing-item-card"
            )
            .forEach(
                function (card) {

                    validateSewingQuantity(card);

                    validateSewingFactories(card);

                    updateAddFactoryButton(card);

                }
            );

    }


    // =====================================================
    // STEP 3 -> STEP 4
    // =====================================================

    $("#nextStep3").on(
        "click",
        function () {

            const cards =
                document.querySelectorAll(
                    ".sewing-item-card"
                );


            if (cards.length === 0) {

                alert(
                    "No sewing assignment item found."
                );

                return;
            }


            let hasError = false;


            // -------------------------------------------------
            // Validate every item
            // -------------------------------------------------

            cards.forEach(
                function (card) {

                    const totalQty =
                        parseFloat(
                            card.dataset.totalQty
                        ) || 0;


                    const assignedQty =
                        calculateAssignedQty(card);


                    // -----------------------------------------
                    // Over quantity
                    // -----------------------------------------

                    if (
                        assignedQty >
                        totalQty
                    ) {

                        hasError = true;

                        return;
                    }


                    // -----------------------------------------
                    // Not fully assigned
                    // -----------------------------------------

                    if (
                        assignedQty !==
                        totalQty
                    ) {

                        hasError = true;

                    }


                    // -----------------------------------------
                    // Factory validation
                    // -----------------------------------------

                    card.querySelectorAll(
                        ".sewing-factory"
                    ).forEach(
                        function (select) {

                            const row =
                                select.closest("tr");


                            const qtyInput =
                                row.querySelector(
                                    ".sewing-production-qty"
                                );


                            const qty =
                                parseFloat(
                                    qtyInput.value
                                ) || 0;


                            // ---------------------------------
                            // Qty entered but factory missing
                            // ---------------------------------

                            if (
                                qty > 0 &&
                                !select.value
                            ) {

                                hasError = true;


                                select.classList.add(
                                    "sewing-invalid"
                                );

                            }

                        }
                    );


                    // -----------------------------------------
                    // Duplicate factory
                    // -----------------------------------------

                    validateSewingFactories(card);


                    if (
                        card.querySelector(
                            ".sewing-invalid"
                        )
                    ) {

                        hasError = true;

                    }

                }
            );


            // -------------------------------------------------
            // Error
            // -------------------------------------------------

            if (hasError) {

                alert(
                    "Please complete the sewing factory assignment for all items.\n\n" +
                    "Each item's Assigned Qty. must exactly match the Order Qty."
                );


                return;

            }


            // -------------------------------------------------
            // Build hidden inputs
            // -------------------------------------------------

            buildSewingAssignmentHiddenInputs();


            // -------------------------------------------------
            // Step 4
            // -------------------------------------------------

            showStep(4);

        }
    );


    // =====================================================
    // BUILD SEWING ASSIGNMENT HIDDEN INPUTS
    // =====================================================

    function buildSewingAssignmentHiddenInputs() {

        const $container =
            $("#sewingAssignmentInputs");


        if (!$container.length) {
            return;
        }


        $container.empty();


        let assignmentIndex = 0;


        document
            .querySelectorAll(
                ".sewing-item-card"
            )
            .forEach(
                function (card) {

                    const itemIndex =
                        card.dataset.itemIndex;


                    const itemName =
                        card.dataset.itemName;


                    card.querySelectorAll(
                        '[data-role="assignmentRows"] tr'
                    ).forEach(
                        function (row) {

                            const factory =
                                row.querySelector(
                                    ".sewing-factory"
                                )?.value || "";


                            const productionQty =
                                row.querySelector(
                                    ".sewing-production-qty"
                                )?.value || "";


                            const smv =
                                row.querySelector(
                                    ".sewing-smv"
                                )?.value || "";


                            const machine =
                                row.querySelector(
                                    ".sewing-machine"
                                )?.value || "";


                            const remarks =
                                row.querySelector(
                                    ".sewing-row-remarks"
                                )?.value || "";


                            // ---------------------------------
                            // Skip empty rows
                            // ---------------------------------

                            if (
                                !factory &&
                                !productionQty
                            ) {

                                return;

                            }


                            // ---------------------------------
                            // Hidden inputs
                            // ---------------------------------

                            appendHiddenInput(
                                $container,
                                `SewingAssignments[${assignmentIndex}].ItemIndex`,
                                itemIndex
                            );


                            appendHiddenInput(
                                $container,
                                `SewingAssignments[${assignmentIndex}].ItemName`,
                                itemName
                            );


                            appendHiddenInput(
                                $container,
                                `SewingAssignments[${assignmentIndex}].Factory`,
                                factory
                            );


                            appendHiddenInput(
                                $container,
                                `SewingAssignments[${assignmentIndex}].ProductionQty`,
                                productionQty
                            );


                            appendHiddenInput(
                                $container,
                                `SewingAssignments[${assignmentIndex}].SMV`,
                                smv
                            );


                            appendHiddenInput(
                                $container,
                                `SewingAssignments[${assignmentIndex}].Machine`,
                                machine
                            );


                            appendHiddenInput(
                                $container,
                                `SewingAssignments[${assignmentIndex}].Remarks`,
                                remarks
                            );


                            assignmentIndex++;

                        }
                    );

                }
            );


        console.log(
            "Sewing Assignment Count:",
            assignmentIndex
        );

    }


    // =====================================================
    // STEP 4 -> STEP 3
    // =====================================================

    $("#previousStep4").on(
        "click",
        function () {

            showStep(3);

        }
    );


    // =====================================================
    // FORM SUBMIT
    // =====================================================

    $("#recapForm").on(
        "submit",
        function (e) {

            console.log(
                "========== FINAL SUBMIT =========="
            );


            // -------------------------------------------------
            // Rebuild Item Details
            // -------------------------------------------------

            buildItemHiddenInputs();


            // -------------------------------------------------
            // Rebuild Recap Details
            // -------------------------------------------------

            const bookingNo =
                $("#BookingNo").val();


            if (bookingNo) {

                buildRecapDetailHiddenInputsFromTable();

            }
            else {

                $("#recapDetailsInputs")
                    .empty();

            }


            // -------------------------------------------------
            // Rebuild Sewing Assignment
            // -------------------------------------------------

            buildSewingAssignmentHiddenInputs();


            // -------------------------------------------------
            // Item Count
            // -------------------------------------------------

            const itemCount =
                $("#itemDetailsInputs")
                    .find(
                        "input[name^='ItemDetails['][name$='.ItemName']"
                    )
                    .length;


            // -------------------------------------------------
            // Detail Count
            // -------------------------------------------------

            const detailCount =
                $("#recapDetailsInputs")
                    .find(
                        "input[name^='Details['][name$='.ItemName']"
                    )
                    .length;


            // -------------------------------------------------
            // Sewing Count
            // -------------------------------------------------

            const sewingCount =
                $("#sewingAssignmentInputs")
                    .find(
                        "input[name^='SewingAssignments['][name$='.Factory']"
                    )
                    .length;


            console.log(
                "Item Count:",
                itemCount
            );


            console.log(
                "Detail Count:",
                detailCount
            );


            console.log(
                "Sewing Assignment Count:",
                sewingCount
            );


            // -------------------------------------------------
            // Item validation
            // -------------------------------------------------

            if (itemCount === 0) {

                e.preventDefault();


                alert(
                    "Please add at least one Item."
                );


                showStep(1);


                return false;

            }


            // -------------------------------------------------
            // Detail validation
            // -------------------------------------------------

            if (
                bookingNo &&
                detailCount === 0
            ) {

                e.preventDefault();


                alert(
                    "No Recap Detail found for the selected Internal Ref."
                );


                showStep(2);


                return false;

            }


            // -------------------------------------------------
            // Sewing validation
            // -------------------------------------------------

            const sewingCards =
                document.querySelectorAll(
                    ".sewing-item-card"
                );


            let sewingError = false;


            sewingCards.forEach(
                function (card) {

                    const totalQty =
                        parseFloat(
                            card.dataset.totalQty
                        ) || 0;


                    const assignedQty =
                        calculateAssignedQty(card);


                    if (
                        assignedQty !==
                        totalQty
                    ) {

                        sewingError = true;

                    }


                    validateSewingFactories(card);


                    if (
                        card.querySelector(
                            ".sewing-invalid"
                        )
                    ) {

                        sewingError = true;

                    }

                }
            );


            if (sewingError) {

                e.preventDefault();


                alert(
                    "Please complete all Sewing Factory assignments before saving."
                );


                showStep(3);


                return false;

            }


            // -------------------------------------------------
            // Final FormData Debug
            // -------------------------------------------------

            const formData =
                new FormData(this);


            console.log(
                "----- FINAL FORM DATA -----"
            );


            for (
                const pair of formData.entries()
            ) {

                console.log(
                    pair[0],
                    "=",
                    pair[1]
                );

            }


            console.log(
                "Submitting form..."
            );


            return true;

        }
    );


    // =====================================================
    // EXISTING ITEM ROWS
    // =====================================================

    if (
        $("#itemsBody tr").length > 0
    ) {

        renumberItems();

        buildItemHiddenInputs();

    }


    // =====================================================
    // EXISTING RECAP DETAIL ROWS
    // =====================================================

    if (
        $("#detailsBody tr").length > 0
    ) {

        renumberRecapDetails();

        buildRecapDetailHiddenInputsFromTable();

    }


    // =====================================================
    // FORMAT QTY
    // =====================================================

    function formatQty(value) {

        return Number(value)
            .toLocaleString(
                "en-US",
                {
                    maximumFractionDigits: 2
                }
            );

    }


    // =====================================================
    // HTML ESCAPE
    // =====================================================

    function escapeHtml(value) {

        if (
            value === null ||
            value === undefined
        ) {

            return "";

        }


        return String(value)
            .replace(
                /&/g,
                "&amp;"
            )
            .replace(
                /</g,
                "&lt;"
            )
            .replace(
                />/g,
                "&gt;"
            )
            .replace(
                /"/g,
                "&quot;"
            )
            .replace(
                /'/g,
                "&#039;"
            );

    }


    // =====================================================
    // HTML ATTRIBUTE ESCAPE
    // =====================================================

    function escapeHtmlAttribute(value) {

        if (
            value === null ||
            value === undefined
        ) {

            return "";

        }


        return String(value)
            .replace(
                /&/g,
                "&amp;"
            )
            .replace(
                /"/g,
                "&quot;"
            )
            .replace(
                /</g,
                "&lt;"
            )
            .replace(
                />/g,
                "&gt;"
            );

    }


    // =====================================================
    // DEBUG
    // =====================================================

    console.log(
        "Recap Info 4-Step JS loaded successfully."
    );

});