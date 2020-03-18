using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class Gun : MonoBehaviour {
		
	public float damage = 10f; //переменная - урон от оружия
	public float range = 100f; //п. - дальность оружия

	public Camera fpsCam; //ссылка на нашу камеру
	public ParticleSystem muzzleFlash; //ссылка на эффект выстрела
	public GameObject impactEffect; //ссылка на эффект попадания

    public int maxAmmo = 10; // переменная, в которой указан максимум патронов
    private int currentAmmo; // переменная для кол-ва патронов на данный момент
    public float reloadTime = 1f; //скорость перезарядки
    private bool isReloading = false; //показывает, активна ли сейчас перезарядка
    public Animator reloadAnimator; //переменная-ссылка на тот объект, с которым сработает анимация

    private void Start() //функция, которая сработает при старте игры
    {
        currentAmmo = maxAmmo; //в начале игры устанавливает максимум патронов
    }

    private void Update () { //функция, которая будет проверять, нажата ли нужная кнопка

        if (isReloading) //если перезарядка не происходит, ведь у нас она равна False
            return; //выполнение функции завершается, переходим к дальнейшему выполнению скрипта
        if (currentAmmo <= 0) //если патронов меньше нуля
        {
            StartCoroutine(Reload()); //начинаем функцию Reload
            return; 
        }
		if (Input.GetButtonDown("Fire1")) { //условие - нажата должна быть ЛКМ (по умолчанию Fire1)
			shoot(); //тогда происходит работа функции Shoot
		}
	}

    IEnumerator Reload() //функция перезарядки
    {
        isReloading = true; // если работает эта функция, мы сразу устанавливаем reloading  в true, чтобы запустить процесс
        Debug.Log("Reloading..."); //выдача в консоль сообщения о перезарядке


        reloadAnimator.SetBool("Reloading", true); //включаем режим перезарядки для аниматора
        yield return new WaitForSeconds(reloadTime - .25f); //проигрывается анимация входа в состояние перезарядки
        reloadAnimator.SetBool("Reloading", false); //выключаем
        yield return new WaitForSeconds(.25f); // проигрывается состояние выхода

        currentAmmo = maxAmmo; //по истечении анимации запас патронов пополняется
        isReloading = false; //выключаем reloading, условие для анимации исчезает
    }


	void shoot () {
		muzzleFlash.Play (); //срабатывание PS выстрела

        currentAmmo--; //то же самое, что currentAmmo = currentAmmo - 1, отнимаем 1 патрон при выстреле

		RaycastHit hit; 
		if (Physics.Raycast (fpsCam.transform.position, fpsCam.transform.forward, out hit, range)) { //переменная, собирающая инфу о выстреле
			Debug.Log (hit.transform); //команда по выдаче 

			Target target = hit.transform.GetComponent<Target> (); //вызываем класс Target, вводим переменную target(цель)
			if (target != null){ //условие, если цель "не отсутствует", то
				target.TakeDamage (damage); //цель получает урон с помощью функции из класса Target


			}

			GameObject impactGO = Instantiate (impactEffect, hit.point, Quaternion.LookRotation (hit.normal)); //создание эффекта попадания  в объект
			Destroy (impactGO, 2f); //через 2 секунды эта PS будет удалена, чтобы не засорять игру и иерархию
		} 
			
	}
}