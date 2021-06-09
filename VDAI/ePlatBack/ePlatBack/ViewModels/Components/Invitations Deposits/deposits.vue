
<template>
    <div>
        <table id="depositsTable" 
         class="table table-striped table-bordered" 
         style="overflow:auto">
            <thead>
                <tr>
                    <th>N°</th>
                    <th>Desarrollo</th>
                    <th>Program</th>
                    <th>Folio</th>
                    <th>Guest</th>
                    <th>Hotel SPI</th>
                    <th>OPC</th>
                    <th>Pick Up</th>
                    <th>Pax</th>
                    <th>Pick Up Notes</th>
                    <th>Comments</th>
                    <th>Delivered</th>
                    <th>Received</th>
                    <th>Money</th>
                    <th>Cash/TDC</th>
                    <th>State</th>
                    <th>Jalador</th>
                    <th>Manifest Folio</th>
                </tr>
            </thead>
            <tbody></tbody>
            <tfoot></tfoot>
        </table>
    </div>
</template>

<script>
    export default ({
        data: function () {
            return {
                invitations:[]     
            }
        },
        methods: {
            setSearchResult() {
                alert('success');
            }
        },
        mounted() {
            console.log('section loaded');
            $('.datetimepicker-input').datetimepicker({
                format: 'YYYY-MM-DD'
            });
        }
    })
</script>

<style>

</style>