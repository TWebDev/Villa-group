<template>
    <div class="mb-5">
        <h1 class="mb-4">Cards Management </h1>
        
        <component :is="$root.currentcomponent" :cards="card"></component>



    </div>
</template>
<script>
    import axios from 'axios';
    import searchcards from './searchCards.vue';
    import newcard from './newCard.vue';

    export default {
        props: ['cards'],
            data: function () {
                return {
                    test: 'SPC single page component',
                    card:'',
                    
                }
        },
        components: {
            'searchcards': searchcards,
            'newcard': newcard,
          
        },
      
        mounted() {
           

            },
        }

</script>
