<script setup lang="ts">
    // This starter template is using Vue 3 <script setup> SFCs
    // Check out https://vuejs.org/api/sfc-script-setup.html#script-setup
    import HomePage from './components/HomePage.vue'
</script>

<template>
    <HomePage />
</template>

<style>
    html {
        color: #fff;
        background-color: #000;
    }

    html,
    button,
    input {
        font-family: 'Courier New', Courier, monospace;
        font-weight: bold;
    }

    #app {
        margin: 0;
        padding: 10px;
        text-align: center;
        -webkit-font-smoothing: antialiased;
        -moz-osx-font-smoothing: grayscale;
    }

    a {
        color: #55ffff;
    }

    label {
        justify-content: center;
        text-align: center;
    }

    @media screen and (max-width: 2400px) {
        html,
        button,
        input {
            font-size: large;
        }

        img {
            width: 220px;
        }

        figcaption {
            width: 258px;
        }

        button {
            padding: 20px;
        }
    }

    @media screen and (max-width: 1200px) {
        html,
        button,
        input {
            font-size: normal;
        }

        img {
            width: 150px;
        }

        figcaption {
            width: 168px;
        }

        button {
            padding: 15px;
        }
    }

    @media screen and (max-width: 600px) {
        html,
        button,
        input {
            font-size: small;
        }

        img {
            width: 100px;
        }

        figcaption {
            width: 118px;
        }

        button {
            padding: 10px;
        }
    }

    img {
        margin: 10px;
    }

    button {
        margin: 10px;
        border-color: #fff;
        color: #000;
        background-color: #ff55ff;
    }

    button:disabled,
    input[type=text]:disabled {
        border-color: #fff;
        background-color: #55ffff;
    }

    input[type=text] {
        margin: 10px;
        padding: 10px;
        border-color: #ff55ff;
        color: #000;
        background-color: #fff;
    }

    .pink {
        color: #ff55ff;
    }

    .info {
        font-size: smaller;
        color: #fff;
    }

    .lighter {
        font-size: smaller;
        font-weight: lighter;
    }

    hr {
        width: 80%;
        height: 1px;
        margin-top: 25px;
        border-color: #fff;
        background-color: #55ffff;
    }

    .message {
        margin: 10px;
        padding: 10px;
        color: #55ffff;
    }

    .message::before {
        content: "<< ";
    }

    .message::after {
        content: " >>";
    }
</style>
