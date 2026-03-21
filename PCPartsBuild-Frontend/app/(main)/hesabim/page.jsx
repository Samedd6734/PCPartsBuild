'use client';

import React, { useState, useEffect } from 'react';
import { api } from '@/lib/api';
import Swal from 'sweetalert2';

/**
 * AccountProfilePage - STRICT CONTENT ONLY RESTORATION
 * Focused exclusively on the profile form as per legacy hesabim.html.
 */
export default function AccountProfilePage() {
    const [lang, setLang] = useState('tr');
    const [loading, setLoading] = useState(true);
    const [isEditing, setIsEditing] = useState(false);
    const [isSaving, setIsSaving] = useState(false);
    
    // Form Data State (Exact Fields from Legacy)
    const [formData, setFormData] = useState({
        fullName: '',
        username: '',
        email: '',
        phoneNumber: ''
    });
    
    // Backup for Cancel Action
    const [originalData, setOriginalData] = useState(null);

    useEffect(() => {
        const savedLang = localStorage.getItem('lang') || 'tr';
        setLang(savedLang);
        fetchUserData();
    }, []);

    const fetchUserData = async () => {
        const userId = localStorage.getItem('userId');
        if (!userId) return;

        setLoading(true);
        try {
            const response = await api.get(`auth/get-user/${userId}`);
            if (response.ok) {
                const data = await response.json();
                const mappedData = {
                    fullName: data.fullName || '',
                    username: data.username || '',
                    email: data.email || '',
                    phoneNumber: data.phoneNumber || ''
                };
                setFormData(mappedData);
                setOriginalData(mappedData);
            }
        } catch (error) {
            console.error('Fetch Error:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleToggleEdit = () => {
        if (isEditing) {
            setFormData(originalData); // Restore original values on cancel
            setIsEditing(false);
        } else {
            setIsEditing(true);
        }
    };

    const handleSave = async (e) => {
        e.preventDefault();
        const userId = localStorage.getItem('userId');
        if (!userId) return;

        setIsSaving(true);
        try {
            const response = await api.put('auth/update-profile', {
                userId,
                ...formData
            });

            const resultData = await response.json();

            if (response.ok) {
                if (resultData.emailPending || resultData.EmailPending) {
                    Swal.fire({
                        title: lang === 'tr' ? 'Onay Gerekli' : 'Confirmation Required',
                        html: lang === 'tr' 
                            ? 'Profil bilgileriniz güncellendi.<br><br><b>Ancak e-posta değişikliği için</b> mailinizi onaylamanız gerekmektedir.'
                            : 'Profile info updated.<br><br><b>Please check your email</b> to confirm your new address.',
                        icon: 'info', background: '#111c22', color: '#fff'
                    });
                } else {
                    Swal.fire({
                        title: lang === 'tr' ? 'Başarılı!' : 'Success!',
                        text: lang === 'tr' ? 'Profil bilgileriniz güncellendi.' : 'Profile updated.',
                        icon: 'success', background: '#111c22', color: '#fff', confirmButtonColor: '#16a3b2'
                    });
                }
                setIsEditing(false);
                fetchUserData();
            } else {
                Swal.fire({ title: 'Hata', text: 'Update failed', icon: 'error', background: '#111c22', color: '#fff' });
            }
        } catch (error) {
            console.error('Update Error:', error);
            Swal.fire({ title: 'Error', text: 'Server error', icon: 'error', background: '#111c22' });
        } finally {
            setIsSaving(false);
        }
    };

    if (loading) {
        return (
            <div className="flex items-center justify-center min-h-[400px]">
                <div className="h-10 w-10 border-4 border-[#16a3b2]/20 border-t-[#16a3b2] rounded-full animate-spin"></div>
            </div>
        );
    }

    return (
        <div className="bg-[#111c22] rounded-2xl p-8 border border-white/5 w-full transition-all duration-300">
            {/* Form Header */}
            <div className="mb-8 flex items-center justify-between border-b border-white/10 pb-6">
                <h3 className="text-xl font-semibold text-white flex items-center gap-2">
                    <span className="material-symbols-outlined text-[#16a3b2]">person</span>
                    <span>{lang === 'tr' ? 'Kişisel Bilgiler' : 'Personal Information'}</span>
                </h3>
                <button 
                    onClick={handleToggleEdit}
                    className={`text-sm font-bold transition-all ${isEditing ? 'text-red-500 hover:text-red-400' : 'text-[#16a3b2] hover:text-cyan-400'}`}
                >
                    {isEditing ? (lang === 'tr' ? 'İptal' : 'Cancel') : (lang === 'tr' ? 'Düzenle' : 'Edit')}
                </button>
            </div>
            
            {/* Profile Form */}
            <form onSubmit={handleSave} className="grid grid-cols-1 gap-8 sm:grid-cols-2">
                
                {/* 1. Ad Soyad */}
                <div className="space-y-3">
                    <label className="text-[11px] font-bold text-white/40 uppercase tracking-widest pl-1">
                        {lang === 'tr' ? 'Ad Soyad' : 'Full Name'}
                    </label>
                    <input 
                        type="text" 
                        disabled={!isEditing}
                        value={formData.fullName}
                        onChange={(e) => setFormData({...formData, fullName: e.target.value})}
                        className="w-full rounded-xl bg-black/20 border border-white/10 px-5 py-3.5 text-white placeholder-white/20 focus:border-[#16a3b2] focus:outline-none focus:ring-1 focus:ring-[#16a3b2] transition-all disabled:opacity-50"
                    />
                </div>

                {/* 2. Kullanıcı Adı */}
                <div className="space-y-3">
                    <label className="text-[11px] font-bold text-white/40 uppercase tracking-widest pl-1">
                        {lang === 'tr' ? 'Kullanıcı Adı' : 'Username'}
                    </label>
                    <input 
                        type="text" 
                        disabled={!isEditing}
                        value={formData.username}
                        onChange={(e) => setFormData({...formData, username: e.target.value})}
                        className="w-full rounded-xl bg-black/20 border border-white/10 px-5 py-3.5 text-white placeholder-white/20 focus:border-[#16a3b2] focus:outline-none focus:ring-1 focus:ring-[#16a3b2] transition-all disabled:opacity-50"
                    />
                </div>

                {/* 3. E-posta */}
                <div className="space-y-3">
                    <label className="text-[11px] font-bold text-white/40 uppercase tracking-widest pl-1">
                        {lang === 'tr' ? 'E-posta' : 'Email'}
                    </label>
                    <input 
                        type="email" 
                        disabled={!isEditing}
                        value={formData.email}
                        onChange={(e) => setFormData({...formData, email: e.target.value})}
                        className="w-full rounded-xl bg-black/20 border border-white/10 px-5 py-3.5 text-white placeholder-white/20 focus:border-[#16a3b2] focus:outline-none focus:ring-1 focus:ring-[#16a3b2] transition-all disabled:opacity-50"
                    />
                </div>

                {/* 4. Telefon */}
                <div className="space-y-3">
                    <label className="text-[11px] font-bold text-white/40 uppercase tracking-widest pl-1">
                        {lang === 'tr' ? 'Telefon' : 'Phone'}
                    </label>
                    <input 
                        type="tel" 
                        disabled={!isEditing}
                        value={formData.phoneNumber}
                        onChange={(e) => setFormData({...formData, phoneNumber: e.target.value})}
                        className="w-full rounded-xl bg-black/20 border border-white/10 px-5 py-3.5 text-white placeholder-white/20 focus:border-[#16a3b2] focus:outline-none focus:ring-1 focus:ring-[#16a3b2] transition-all disabled:opacity-50"
                    />
                </div>
                
                {/* Save Action Block */}
                <div className="sm:col-span-2 flex justify-end pt-6">
                    <button 
                        type="submit" 
                        disabled={!isEditing || isSaving} 
                        className={`min-w-[200px] rounded-xl bg-[#16a3b2] px-8 py-3.5 text-sm font-black text-white shadow-lg shadow-[#16a3b2]/10 hover:brightness-110 active:scale-95 transition-all ${(!isEditing || isSaving) ? 'opacity-50 cursor-not-allowed' : ''}`}
                    >
                        {isSaving 
                            ? (lang === 'tr' ? 'Kaydediliyor...' : 'Saving...') 
                            : (lang === 'tr' ? 'Değişiklikleri Kaydet' : 'Save Changes')
                        }
                    </button>
                </div>
            </form>
        </div>
    );
}
