import styles from './button.module.css'
function ButtonM({action}) {
    return (
        <button className={styles['button-ingreso']} onClick={() => { action() }}>
            Ingresar
        </button>
  );
}

export default ButtonM;