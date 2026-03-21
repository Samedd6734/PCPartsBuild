'use client';

import React, { useState, useEffect } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { api } from '@/lib/api';
import { translations } from '@/lib/translations';
import Swal from 'sweetalert2';

/**
 * LoginPage Component - SHARP & SYNCED EDITION
 * High-fidelity authentication flow with zero-blur for maximum video clarity.
 */
export default function LoginPage() {
    const router = useRouter();
    const [lang, setLang] = useState('tr');
    const [showPassword, setShowPassword] = useState(false);
    const [loading, setLoading] = useState(false);
    const [isLangDropdownOpen, setIsLangDropdownOpen] = useState(false);
    const [formData, setFormData] = useState({
        loginIdentifier: '',
        password: ''
    });

    useEffect(() => {
        const savedLang = localStorage.getItem('lang') || 'tr';
        setLang(savedLang);
        document.documentElement.lang = savedLang;

        const handleStorageChange = () => {
            const currentLang = localStorage.getItem('lang') || 'tr';
            setLang(currentLang);
            document.documentElement.lang = currentLang;
        };
        window.addEventListener('storage', handleStorageChange);
        return () => window.removeEventListener('storage', handleStorageChange);
    }, []);

    const t = translations[lang];

    const toggleLanguage = (newLang) => {
        setLang(newLang);
        localStorage.setItem('lang', newLang);
        document.documentElement.lang = newLang;
        setIsLangDropdownOpen(false);
        window.dispatchEvent(new Event('storage'));
    };

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleLogin = async (e) => {
        e.preventDefault();
        setLoading(true);

        try {
            const response = await api.post('auth/login', {
                loginIdentifier: formData.loginIdentifier,
                password: formData.password
            });

            const result = await response.json();

            if (response.ok) {
                localStorage.setItem('loggedInUser', result.username);
                if (result.userId) localStorage.setItem('userId', result.userId);
                if (result.token) localStorage.setItem('token', result.token);

                Swal.fire({
                    title: lang === 'tr' ? 'Hoş Geldiniz!' : 'Welcome!',
                    text: lang === 'tr' ? 'Giriş başarılı. Yönlendiriliyorsunuz...' : 'Login successful. Redirecting...',
                    icon: 'success',
                    background: '#0d1117', color: '#fff', confirmButtonColor: '#16a3b2',
                    timer: 1000, timerProgressBar: true, showConfirmButton: false
                }).then(() => {
                    window.dispatchEvent(new Event('storage'));
                    router.push('/');
                });
            } else {
                Swal.fire({
                    title: lang === 'tr' ? 'Giriş Başarısız' : 'Login Failed',
                    text: result.message || (lang === 'tr' ? 'Kullanıcı adı veya şifre hatalı.' : 'Invalid username or password.'),
                    icon: 'error',
                    background: '#0d1117', color: '#fff', confirmButtonColor: '#d33',
                    confirmButtonText: lang === 'tr' ? 'Tekrar Dene' : 'Try Again'
                });
            }
        } catch (error) {
            console.error('Login Error:', error);
            Swal.fire({
                title: lang === 'tr' ? 'Bağlantı Hatası' : 'Connection Error',
                text: lang === 'tr' ? 'Sunucuya ulaşılamadı. Lütfen internet bağlantınızı kontrol edin.' : 'Could not reach server. Please check your connection.',
                icon: 'error', background: '#0d1117', color: '#fff', confirmButtonColor: '#d33'
            });
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="font-auth font-display transition-default overflow-hidden bg-background-dark min-h-screen relative">
            {/* 1. Background Video - SHARP & CLEAR */}
            <video autoPlay id="bg-video" loop muted playsInline className="fixed inset-0 w-full h-full object-cover z-0 brightness-[0.5]">
                <source src="/Videos/Video1.webm" type="video/webm" />
                Your browser does not support the video tag.
            </video>

            {/* 2. Top Navigation - SHARED */}
            <Link 
                href="/" 
                className="absolute top-4 left-4 z-50 w-12 h-12 rounded-full bg-black/40 border border-white/10 flex items-center justify-center text-white hover:bg-black/60 transition-all hover:scale-105"
                title="Home"
            >
                <span className="material-symbols-outlined text-xl">home</span>
            </Link>

            <div className="absolute top-4 right-4 z-50 flex items-center gap-2">
                <div className="relative">
                    <button 
                        onClick={() => setIsLangDropdownOpen(!isLangDropdownOpen)}
                        className="flex cursor-pointer items-center justify-center overflow-hidden rounded-lg h-10 w-10 bg-white/10 text-white transition-colors hover:bg-white/20 border border-white/10"
                    >
                        <span className="material-symbols-outlined text-xl">language</span>
                    </button>
                    {isLangDropdownOpen && (
                        <div className="absolute right-0 top-full pt-1 w-32 bg-gray-900 border border-white/10 rounded-lg shadow-2xl overflow-hidden z-30">
                            <button onClick={() => toggleLanguage('tr')} className="block w-full text-left px-4 py-2 text-sm text-gray-200 font-medium hover:bg-primary/80 hover:text-white transition-colors">Türkçe (TR)</button>
                            <button onClick={() => toggleLanguage('en')} className="block w-full text-left px-4 py-2 text-sm text-gray-200 font-medium hover:bg-primary/80 hover:text-white transition-colors">English (EN)</button>
                        </div>
                    )}
                </div>
            </div>

            {/* 3. Main Split Container - SHARP SYNC */}
            <div className="relative z-10 min-h-screen grid grid-cols-1 lg:grid-cols-2 bg-transparent">
                
                {/* Branding Side (Lg only) */}
                <div className="hidden lg:flex flex-col items-center justify-center text-center px-8 bg-transparent animate-fade-in">
                    <img src="/Images/logo.webp" alt="PCPartsBuild Logo" className="h-48 drop-shadow-2xl" />
                    <p className="text-gray-300 mt-6 max-w-sm text-lg font-medium tracking-wide drop-shadow-lg">
                        {t["auth-welcome-subtitle"] || "Hayalinizdeki Bilgisayarı hassasiyet ve güvenle tasarlayın."}
                    </p>
                </div>

                {/* Right Form Side - SHARP THEME (No Blur) */}
                <div className="relative flex items-center justify-center p-4 lg:p-12 bg-background-dark/95 border-l border-gray-800 shadow-2xl">
                    
                    <div className="relative w-full max-w-md p-6 sm:p-10 bg-gray-900/40 rounded-[2.5rem] border border-white/5 animate-fade-in transition-all">
                        <header className="text-center lg:text-left mb-8">
                            <h1 className="text-3xl font-bold text-white tracking-tight">
                                {t["auth-login-title"] || "Hoşgeldiniz"}
                            </h1>
                            <p className="text-gray-400 mt-2 text-sm italic">
                                {t["auth-login-subtitle"] || "Özel bilgisayar yolculuğunuza bugün başlayın."}
                            </p>
                        </header>

                        <main>
                            <form onSubmit={handleLogin} className="space-y-6">
                                <div className="space-y-1.5">
                                    <label className="block text-sm font-semibold text-gray-300 ml-1" htmlFor="loginIdentifier">
                                        {t["label-email-user"] || "Kullanıcı"}
                                    </label>
                                    <input 
                                        className="w-full bg-white/5 border border-white/10 rounded-2xl focus:ring-2 focus:ring-primary focus:border-primary text-white placeholder:text-gray-600 transition-all h-14 px-5 outline-none active:bg-white/10 focus:bg-white/10" 
                                        name="loginIdentifier" 
                                        placeholder={t["placeholder-email-user"] || "Kullanıcı Adı"}
                                        type="text"
                                        required
                                        value={formData.loginIdentifier}
                                        onChange={handleChange}
                                    />
                                </div>
                                
                                <div className="space-y-1.5">
                                    <label className="block text-sm font-semibold text-gray-300 ml-1" htmlFor="password">
                                        {t["label-password"] || "Şifre"}
                                    </label>
                                    <div className="relative">
                                        <input 
                                            className="w-full bg-white/5 border border-white/10 rounded-2xl focus:ring-2 focus:ring-primary focus:border-primary text-white placeholder:text-gray-600 transition-all h-14 pl-5 pr-12 outline-none active:bg-white/10 focus:bg-white/10" 
                                            name="password" 
                                            placeholder="••••••••" 
                                            type={showPassword ? "text" : "password"}
                                            required
                                            value={formData.password}
                                            onChange={handleChange}
                                        />
                                        <button 
                                            type="button" 
                                            onClick={() => setShowPassword(!showPassword)} 
                                            className="absolute right-4 top-1/2 -translate-y-1/2 text-gray-500 hover:text-white transition-colors p-1"
                                        >
                                            <span className="material-symbols-outlined text-xl">
                                                {showPassword ? "visibility_off" : "visibility"}
                                            </span>
                                        </button>
                                    </div>
                                </div>
                                
                                <button 
                                    className="w-full bg-primary text-white font-bold py-5 px-6 rounded-2xl shadow-xl shadow-primary/20 flex items-center justify-center gap-2 mt-4 hover:bg-primary/90 transition-all active:scale-[0.98] disabled:opacity-50" 
                                    type="submit"
                                    disabled={loading}
                                >
                                    {loading ? (
                                        <div className="h-5 w-5 border-2 border-white/20 border-t-white rounded-full animate-spin"></div>
                                    ) : (t["action-login"] || "Giriş Yap")}
                                </button>
                            </form>

                            <div className="text-right mt-5">
                                <Link href="/sifremi-unuttum" className="text-sm font-medium text-primary hover:underline transition-all">
                                    {t["link-forgot-password"] || "Şifreni mi unuttun?"}
                                </Link>
                            </div>
                        </main>

                        <footer className="text-center mt-10 pt-8 border-t border-white/5">
                            <p className="text-sm text-gray-400">
                                {t["link-no-account"] || "Henüz Kayıt olmadın mı?"}{" "}
                                <Link href="/kaydol" className="font-bold text-primary hover:underline ml-1">
                                    {t["link-signup"] || "Kayıt Ol"}
                                </Link>
                            </p>
                        </footer>
                    </div>

                    {/* Shared Page Footer */}
                    <footer className="absolute bottom-6 w-full text-center px-4"> 
                        <p className="text-xs text-gray-500 font-medium tracking-wide"> 
                            © 2025 PCPartsBuild. {lang === 'tr' ? 'Tüm Hakları Saklıdır.' : 'All Rights Reserved.'}
                            <span className="mx-2 opacity-30">|</span> 
                            {lang === 'tr' ? 'Bize Ulaşın:' : 'Contact Us:'}{" "}
                            <a className="text-primary hover:underline" href="mailto:pcpartsbuild@gmail.com">pcpartsbuild@gmail.com</a>
                        </p>
                    </footer>
                </div>
            </div>
        </div>
    );
}
