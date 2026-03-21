'use client';

import React, { useState, useEffect } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { api } from '@/lib/api';
import { translations } from '@/lib/translations';
import Swal from 'sweetalert2';

/**
 * ForgotPasswordPage Component - SHARP & SYNCED EDITION
 * High-fidelity authentication flow with zero-blur for maximum video clarity.
 */
export default function ForgotPasswordPage() {
    const router = useRouter();
    const [lang, setLang] = useState('tr');
    const [step, setStep] = useState(1); // 1: Email, 2: Code
    const [loading, setLoading] = useState(false);
    const [isLangDropdownOpen, setIsLangDropdownOpen] = useState(false);
    const [email, setEmail] = useState('');
    const [code, setCode] = useState('');

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

    const handleSendCode = async (e) => {
        e.preventDefault();
        setLoading(true);

        try {
            const response = await api.post('auth/forgot-password', { email });

            if (response.ok) {
                setStep(2);
                Swal.fire({
                    icon: 'success',
                    title: lang === 'tr' ? 'Kod Gönderildi' : 'Code Sent',
                    text: lang === 'tr' ? 'Lütfen mail kutunuzu kontrol edin.' : 'Please check your email inbox.',
                    background: '#0d1117', color: '#fff', confirmButtonColor: '#16a3b2',
                    timer: 2000, timerProgressBar: true
                });
            } else {
                Swal.fire({ 
                    icon: 'error', 
                    title: lang === 'tr' ? 'Hata' : 'Error', 
                    text: lang === 'tr' ? 'Kod gönderilemedi.' : 'Could not send code.', 
                    background: '#0d1117', color: '#fff' 
                });
            }
        } catch (error) {
            console.error('Forgot Password Error:', error);
            Swal.fire({ 
                icon: 'error', 
                title: lang === 'tr' ? 'Hata' : 'Error', 
                text: lang === 'tr' ? 'Sunucuya ulaşılamadı. Lütfen bağlantınızı kontrol edin.' : 'Could not reach server. Please check your connection.', 
                background: '#0d1117', color: '#fff' 
            });
        } finally {
            setLoading(false);
        }
    };

    const handleVerifyCode = async (e) => {
        e.preventDefault();
        setLoading(true);

        try {
            const response = await api.post('auth/verify-code', { email, code });
            const result = await response.json();

            if (response.ok && result.token) {
                router.push(`/sifre-sifirla?email=${encodeURIComponent(email)}&token=${encodeURIComponent(result.token)}`);
            } else {
                Swal.fire({
                    icon: 'error',
                    title: lang === 'tr' ? 'Hatalı Kod' : 'Invalid Code',
                    text: result.message || (lang === 'tr' ? 'Girdiğiniz kod yanlış.' : 'The code you entered is incorrect.'),
                    background: '#0d1117', color: '#fff', confirmButtonColor: '#d33'
                });
            }
        } catch (error) {
            console.error('Verify Code Error:', error);
            Swal.fire({ 
                icon: 'error', 
                title: lang === 'tr' ? 'Hata' : 'Error', 
                text: lang === 'tr' ? 'Sunucu hatası.' : 'Server error.', 
                background: '#0d1117', color: '#fff' 
            });
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="font-auth bg-background-dark font-display text-gray-300 transition-default overflow-hidden min-h-screen relative">
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
                <span className="material-symbols-outlined text-xl"> home </span>
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
                                {lang === 'tr' ? 'Şifre Sıfırlama' : 'Password Reset'}
                            </h1>
                            <p className="text-gray-400 mt-2 text-sm italic">
                                {step === 1 
                                    ? (lang === 'tr' ? 'Hesabınıza bağlı e-posta adresini girin.' : 'Enter the email address associated with your account.')
                                    : (lang === 'tr' ? 'Mailinize gelen 6 haneli kodu girin.' : 'Enter the 6-digit code sent to your email.')
                                }
                            </p>
                        </header>

                        <main>
                            {step === 1 ? (
                                <form onSubmit={handleSendCode} className="space-y-6">
                                    <div className="space-y-1.5">
                                        <label className="block text-sm font-semibold text-gray-300 ml-1" htmlFor="email">
                                            {t["label-email"] || "Email"}
                                        </label>
                                        <input 
                                            className="w-full bg-white/5 border border-white/10 rounded-2xl focus:ring-2 focus:ring-primary focus:border-primary text-white placeholder:text-gray-600 transition-all h-14 px-5 outline-none active:bg-white/10 focus:bg-white/10" 
                                            id="email" name="email" placeholder="example@mail.com" type="email" required
                                            value={email} onChange={(e) => setEmail(e.target.value)}
                                        />
                                    </div>
                                    <button 
                                        className="w-full bg-primary text-white font-bold py-5 px-6 rounded-2xl shadow-xl shadow-primary/20 flex items-center justify-center gap-2 mt-4 hover:bg-primary/90 transition-all active:scale-[0.98] disabled:opacity-50"
                                        type="submit" disabled={loading}
                                    >
                                        {loading ? (
                                            <div className="h-5 w-5 border-2 border-white/20 border-t-white rounded-full animate-spin"></div>
                                        ) : (lang === 'tr' ? 'Doğrulama Kodu Gönder' : 'Send Verification Code')}
                                    </button>
                                </form>
                            ) : (
                                <form onSubmit={handleVerifyCode} className="space-y-6">
                                    <div className="text-center mb-6 py-3 px-4 bg-white/5 rounded-xl border border-white/5">
                                        <span className="text-sm text-gray-400">
                                            {lang === 'tr' ? 'Kod şuraya gönderildi:' : 'Code sent to:'}{" "}
                                            <span className="text-white font-bold">{email}</span>
                                        </span>
                                    </div>
                                    <div className="space-y-1.5">
                                        <label className="block text-sm font-semibold text-gray-300 ml-1" htmlFor="code">
                                            {lang === 'tr' ? 'Doğrulama Kodu' : 'Verification Code'}
                                        </label>
                                        <input 
                                            className="w-full bg-white/5 border border-white/10 rounded-2xl focus:ring-2 focus:ring-primary focus:border-primary text-white text-center text-3xl font-bold tracking-[0.5em] placeholder:text-gray-700 transition-all h-16 px-5 outline-none active:bg-white/10 focus:bg-white/10" 
                                            id="code" name="code" placeholder="******" type="text" maxLength="6" required
                                            value={code} onChange={(e) => setCode(e.target.value)}
                                        />
                                    </div>
                                    <button 
                                        className="w-full bg-primary text-white font-bold py-5 px-6 rounded-2xl shadow-xl shadow-primary/20 flex items-center justify-center gap-2 mt-4 hover:bg-primary/90 transition-all active:scale-[0.98] disabled:opacity-50"
                                        type="submit" disabled={loading}
                                    >
                                        {loading ? (
                                            <div className="h-5 w-5 border-2 border-white/20 border-t-white rounded-full animate-spin"></div>
                                        ) : (lang === 'tr' ? 'Kodu Doğrula' : 'Verify Code')}
                                    </button>
                                    
                                    <button 
                                        type="button" 
                                        onClick={() => { setStep(1); setCode(''); }}
                                        className="w-full text-sm font-medium text-gray-500 hover:text-white transition-colors mt-2 text-center"
                                    >
                                        {lang === 'tr' ? 'E-posta adresini değiştir' : 'Change email address'}
                                    </button>
                                </form>
                            )}
                        </main>

                        <footer className="text-center mt-10 pt-8 border-t border-white/5">
                            <Link className="text-sm font-bold text-primary hover:underline transition-all" href="/giris">
                                {lang === 'tr' ? 'Giriş Sayfasına Dön' : 'Back to Login'}
                            </Link>
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
