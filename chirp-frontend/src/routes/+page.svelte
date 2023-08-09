<script lang='ts'>
</script>

<svelte:head>
  <title>Chirp!</title>
</svelte:head>

<main class='main-wrapper'>
  <div class='login-wrapper'>
    <div class='logo-panel'></div>
    <div class='login-panel'>
      <div class='inner'>
        <div class='chirp'>Chirp</div>
        <div class='cta-join'>Join today</div>
        <form action='/register' method='POST'>
          <div class='form-group'>
            <input type='text' name='name' placeholder='Name' />
          </div>
          <div class='form-group'>
            <input type='email' name='email' placeholder='Email' />
          </div>
          <div class='form-group'>
            <input type='password' name='password' placeholder='Password' />
          </div>
          <div class='form-group'>
            <input type='password' name='password_confirmation' placeholder='Confirm Password' />
          </div>
          <div class='form-group'>
            <button type='submit'>Sign up</button>
          </div>
        </form>
        <div class='cta-login'>Already have an account? <a href='/login'>Log in</a></div>
      </div>
    </div>
  </div>
  <footer>
    <span>© 2023 Chirp</span>
  </footer>
</main>

<style lang='scss'>
  .main-wrapper {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    height: 100vh;

    .login-wrapper {
      display: flex;
      flex-grow: 1;
      flex-direction: row;
      align-items: stretch;
      width: 100%;
      height: 100%;

      .logo-panel {
        height: 100%;
        flex-grow: 1;
        flex-shrink: 1;
      }

      .login-panel {
        flex-shrink: 0;
        flex-grow: 0;
        min-width: 45vw;
        height: 100%;
        display: flex;
        flex-direction: column;
        justify-content: center;

        .inner {
          display: flex;
          flex-direction: column;
          min-width: 437px;
          max-width: 760px;

          padding: 32px 48px;
          justify-content: center;

          .chirp {
            font-size: 40px;
            font-weight: 700;
            color: rgb(29, 161, 242);
            margin-bottom: 8px;
          }

          .cta-join {
            font-size: 19px;
            font-weight: 700;
            color: rgb(255, 255, 255);
            margin-bottom: 16px;
          }

          .form-group {
            margin-bottom: 16px;
          }

          input {
            width: 100%;
            max-width: calc(300px - 16px * 2 - 1px * 2);
            padding: 12px 16px;
            border-radius: 4px;
            border: 1px solid rgb(136, 153, 166);
            background-color: rgb(21, 24, 28);
            color: rgb(255, 255, 255);
            font-size: 15px;
            font-weight: 400;
            outline: none;
            transition: border-color 0.2s ease-in-out 0s;
          }

          input:focus {
            border-color: rgb(29, 161, 242);
          }

          button {
            width: 100%;
            max-width: 300px;
            padding: 12px 16px;
            border-radius: 4px;
            border: 1px solid rgb(29, 161, 242);
            background-color: rgb(29, 161, 242);
            color: rgb(255, 255, 255);
            font-size: 15px;
            font-weight: 700;
            outline: none;
            transition: background-color 0.2s ease-in-out 0s;
            cursor: pointer;
          }

          button:hover {
            background-color: rgb(26, 145, 218);
            border-color: rgb(26, 145, 218);
          }

          button:active {
            background-color: rgb(21, 122, 183);
            border-color: rgb(21, 122, 183);
          }

          .cta-login {
            margin-top: 16px;
            font-size: 15px;
            font-weight: 400;
            color: rgb(136, 153, 166);
            a {
              color: rgb(29, 161, 242);
              text-decoration: none;

              &:hover {
                text-decoration: underline;
              }
            }
          }
        }
      }
    }

    footer {
      flex-grow: 0;
      padding: 12px 16px;
      color: rgb(113, 118, 123);
      font-size: 13px;
      font-weight: 400;
    }
  }
</style>
